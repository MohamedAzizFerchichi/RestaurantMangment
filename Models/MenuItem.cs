using System.ComponentModel.DataAnnotations;

namespace RestaurantManagement.Models;

public class MenuItem
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }
    
    [Required]
    public string Category { get; set; } = string.Empty; // Appetizer, Main Course, Dessert, Beverage, etc.
    
    public string? ImageUrl { get; set; }
    
    public bool IsAvailable { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public List<OrderItem> OrderItems { get; set; } = new();
} 