using System.ComponentModel.DataAnnotations;

namespace RestaurantManagementApp.Shared.Models;

public class OrderItem
{
    public int Id { get; set; }

    [Required]
    public int OrderId { get; set; }

    [Required]
    public int MenuItemId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal UnitPrice { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Subtotal { get; set; }

    public string? SpecialInstructions { get; set; }

    public Order Order { get; set; } = null!;
    public MenuItem MenuItem { get; set; } = null!;
} 