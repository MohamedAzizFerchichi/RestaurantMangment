using System.ComponentModel.DataAnnotations;
using RestaurantManagement.Shared.Enums;

namespace RestaurantManagement.Shared.Models;

public class Plate
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    public string? ImageUrl { get; set; }

    [Required]
    public PlateType Category { get; set; }

    public bool IsAvailable { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public List<OrderItem> OrderItems { get; set; } = new();
} 