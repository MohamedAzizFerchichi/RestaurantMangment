using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using RestaurantManagement.Server.Data;
using RestaurantManagement.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.IIS;
using Microsoft.AspNetCore.StaticFiles;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure file upload limits
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = 5242880; // 5MB in bytes
});

builder.Services.Configure<FormOptions>(options =>
{
    options.ValueLengthLimit = 5242880; // 5MB in bytes
    options.MultipartBodyLengthLimit = 5242880; // 5MB in bytes
    options.MemoryBufferThreshold = 5242880; // 5MB in bytes
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// Add DbContext
builder.Services.AddDbContext<RestaurantDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity
builder.Services.AddIdentity<User, IdentityRole>(options => {
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    
    // User settings
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
})
    .AddEntityFrameworkStores<RestaurantDbContext>()
    .AddDefaultTokenProviders();

// Add Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]))
    };
    options.Events = new JwtBearerEvents
    {
        OnChallenge = async context =>
        {
            // Handle unauthorized requests
            context.HandleResponse();
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            var result = System.Text.Json.JsonSerializer.Serialize("You are not authorized");
            await context.Response.WriteAsync(result);
        }
    };
});

var app = builder.Build();

// Ensure wwwroot directory exists
var wwwrootPath = Path.Combine(app.Environment.ContentRootPath, "wwwroot");
if (!Directory.Exists(wwwrootPath))
{
    Directory.CreateDirectory(wwwrootPath);
}

// Ensure images/plates directory exists
var imagesPath = Path.Combine(wwwrootPath, "images", "plates");
if (!Directory.Exists(imagesPath))
{
    Directory.CreateDirectory(imagesPath);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

// Configure CORS - must be before auth
app.UseCors("AllowAll");

// Configure static files with custom options
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        // Cache static files for 1 day
        ctx.Context.Response.Headers.Append(
            "Cache-Control", $"public, max-age=86400");
    }
});

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Apply migrations and seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<RestaurantDbContext>();
        var userManager = services.GetRequiredService<UserManager<User>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var logger = services.GetRequiredService<ILogger<Program>>();
        
        logger.LogInformation("Starting database migration and seeding process...");
        
        // Ensure database is created
        context.Database.EnsureCreated();
        logger.LogInformation("Database created or exists.");

        // Create Admin role if it doesn't exist
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            logger.LogInformation("Creating Admin role...");
            var roleResult = await roleManager.CreateAsync(new IdentityRole("Admin"));
            if (!roleResult.Succeeded)
            {
                logger.LogError("Failed to create Admin role: " + string.Join(", ", roleResult.Errors.Select(e => e.Description)));
            }
        }

        // Create User role if it doesn't exist
        if (!await roleManager.RoleExistsAsync("User"))
        {
            logger.LogInformation("Creating User role...");
            var roleResult = await roleManager.CreateAsync(new IdentityRole("User"));
            if (!roleResult.Succeeded)
            {
                logger.LogError("Failed to create User role: " + string.Join(", ", roleResult.Errors.Select(e => e.Description)));
            }
        }

        // Seed admin user
        var adminEmail = "admin@restaurant.com";
        logger.LogInformation($"Checking for admin user with email: {adminEmail}");
        
        // First check if user exists by email
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            logger.LogInformation("Admin user not found. Creating new admin user...");
            
            // Clear any existing users with same normalized email to avoid conflicts
            var existingUsers = await context.Users
                .Where(u => u.NormalizedEmail == adminEmail.ToUpperInvariant())
                .ToListAsync();
                
            if (existingUsers.Any())
            {
                foreach (var user in existingUsers)
                {
                    context.Users.Remove(user);
                }
                await context.SaveChangesAsync();
            }
            
            adminUser = new User
            {
                UserName = adminEmail,
                NormalizedUserName = adminEmail.ToUpperInvariant(),
                Email = adminEmail,
                NormalizedEmail = adminEmail.ToUpperInvariant(),
                EmailConfirmed = true,
                FirstName = "Admin",
                LastName = "User",
                Role = "Admin",
                CreatedAt = DateTime.UtcNow,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await userManager.CreateAsync(adminUser, "Admin123!");
            if (result.Succeeded)
            {
                logger.LogInformation("Admin user created successfully.");
                var roleResult = await userManager.AddToRoleAsync(adminUser, "Admin");
                if (roleResult.Succeeded)
                {
                    logger.LogInformation("Admin role assigned successfully.");
                }
                else
                {
                    logger.LogError("Failed to assign Admin role: " + string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                }
            }
            else
            {
                logger.LogError("Failed to create admin user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
        else
        {
            logger.LogInformation("Admin user already exists. Checking role...");
            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                logger.LogInformation("Assigning Admin role to existing user...");
                var roleResult = await userManager.AddToRoleAsync(adminUser, "Admin");
                if (!roleResult.Succeeded)
                {
                    logger.LogError("Failed to assign Admin role: " + string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                }
            }
            else
            {
                logger.LogInformation("Admin user already has Admin role.");
            }
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
        throw; // Re-throw to ensure the error is visible
    }
}

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
