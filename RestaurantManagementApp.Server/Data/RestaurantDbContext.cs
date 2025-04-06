using Microsoft.EntityFrameworkCore;
using RestaurantManagementApp.Shared.Models;

namespace RestaurantManagementApp.Server.Data;

public class RestaurantDbContext : DbContext
{
    public RestaurantDbContext(DbContextOptions<RestaurantDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderItem> OrderItems { get; set; } = null!;
    public DbSet<MenuItem> MenuItems { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Configure Order entity
        modelBuilder.Entity<Order>()
            .HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure OrderItem entity
        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => o.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => o.MenuItem)
            .WithMany(m => m.OrderItems)
            .HasForeignKey(oi => oi.MenuItemId)
            .OnDelete(DeleteBehavior.Restrict);

        // Seed initial data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Add admin user
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                FirstName = "Admin",
                LastName = "User",
                Email = "admin@restaurant.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                Role = "Admin",
                CreatedAt = DateTime.UtcNow
            }
        );

        // Add sample menu items
        modelBuilder.Entity<MenuItem>().HasData(
            new MenuItem
            {
                Id = 1,
                Name = "Margherita Pizza",
                Description = "Classic tomato sauce, mozzarella, and basil",
                Price = 12.99m,
                Category = "Pizza",
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow
            },
            new MenuItem
            {
                Id = 2,
                Name = "Caesar Salad",
                Description = "Romaine lettuce, croutons, parmesan, and caesar dressing",
                Price = 8.99m,
                Category = "Salad",
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow
            },
            new MenuItem
            {
                Id = 3,
                Name = "Chocolate Cake",
                Description = "Rich chocolate cake with chocolate ganache",
                Price = 6.99m,
                Category = "Dessert",
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow
            }
        );
    }
} 