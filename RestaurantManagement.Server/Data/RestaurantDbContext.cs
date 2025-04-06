using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Shared.Models;

namespace RestaurantManagement.Server.Data;

public class RestaurantDbContext : IdentityDbContext<User>
{
    private static readonly DateTime _seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public RestaurantDbContext(DbContextOptions<RestaurantDbContext> options)
        : base(options)
    {
    }

    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderItem> OrderItems { get; set; } = null!;
    public DbSet<MenuItem> MenuItems { get; set; } = null!;
    public DbSet<Plate> Plates { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Configure Plate entity
        modelBuilder.Entity<Plate>(entity =>
        {
            entity.Property(p => p.Id)
                .ValueGeneratedOnAdd();
            
            entity.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(500);
            
            entity.Property(p => p.Price)
                .HasColumnType("decimal(18,2)");
            
            entity.Property(p => p.Category)
                .IsRequired();
            
            entity.Property(p => p.ImageUrl)
                .HasMaxLength(500);
            
            entity.Property(p => p.IsAvailable)
                .HasDefaultValue(true);
            
            entity.Property(p => p.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // Configure Order entity
        modelBuilder.Entity<Order>()
            .HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure OrderItem entity
        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Plate)
            .WithMany(p => p.OrderItems)
            .HasForeignKey(oi => oi.PlateId)
            .OnDelete(DeleteBehavior.Restrict);

        // Seed initial menu items
        modelBuilder.Entity<MenuItem>().HasData(
            new MenuItem
            {
                Id = 1,
                Name = "Margherita Pizza",
                Description = "Classic pizza with tomato sauce, mozzarella, and basil",
                Price = 12.99m,
                Category = "Main Course",
                IsAvailable = true,
                CreatedAt = _seedDate
            },
            new MenuItem
            {
                Id = 2,
                Name = "Caesar Salad",
                Description = "Fresh romaine lettuce with Caesar dressing, croutons, and parmesan",
                Price = 8.99m,
                Category = "Appetizer",
                IsAvailable = true,
                CreatedAt = _seedDate
            },
            new MenuItem
            {
                Id = 3,
                Name = "Chocolate Cake",
                Description = "Rich chocolate cake with chocolate ganache",
                Price = 6.99m,
                Category = "Dessert",
                IsAvailable = true,
                CreatedAt = _seedDate
            },
            new MenuItem
            {
                Id = 4,
                Name = "Soft Drink",
                Description = "Choice of Coke, Sprite, or Fanta",
                Price = 2.99m,
                Category = "Beverage",
                IsAvailable = true,
                CreatedAt = _seedDate
            }
        );
    }
}
