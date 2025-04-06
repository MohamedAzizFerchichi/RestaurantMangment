using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Models;

namespace RestaurantManagement.Data;

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
            .OnDelete(DeleteBehavior.Restrict);
        
        // Configure OrderItem entity
        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => o.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => o.MenuItem)
            .WithMany(mi => mi.OrderItems)
            .HasForeignKey(oi => oi.MenuItemId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Seed data
        SeedData(modelBuilder);
    }
    
    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed admin user
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                FirstName = "Admin",
                LastName = "User",
                Email = "admin@restaurant.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Role = "Admin",
                CreatedAt = DateTime.UtcNow
            }
        );
        
        // Seed menu items
        modelBuilder.Entity<MenuItem>().HasData(
            new MenuItem
            {
                Id = 1,
                Name = "Margherita Pizza",
                Description = "Classic pizza with tomato sauce, mozzarella, and basil",
                Price = 12.99m,
                Category = "Main Course",
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow
            },
            new MenuItem
            {
                Id = 2,
                Name = "Caesar Salad",
                Description = "Fresh romaine lettuce with Caesar dressing, croutons, and parmesan",
                Price = 8.99m,
                Category = "Appetizer",
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
            },
            new MenuItem
            {
                Id = 4,
                Name = "Soft Drink",
                Description = "Choice of Coke, Sprite, or Fanta",
                Price = 2.99m,
                Category = "Beverage",
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow
            }
        );
    }
} 