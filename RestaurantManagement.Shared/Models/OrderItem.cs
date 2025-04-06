using System.ComponentModel.DataAnnotations;

namespace RestaurantManagement.Shared.Models;

public class OrderItem
{
    public int Id { get; set; }

    [Required]
    public int OrderId { get; set; }

    [Required]
    public int PlateId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal UnitPrice { get; set; }

    public string? Notes { get; set; }

    // Navigation properties
    public Order Order { get; set; } = null!;
    public Plate Plate { get; set; } = null!;
} 