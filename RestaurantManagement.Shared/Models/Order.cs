using System.ComponentModel.DataAnnotations;

namespace RestaurantManagement.Shared.Models;

public class Order
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    public DateTime OrderDate { get; set; }

    [Required]
    public string Status { get; set; } = "Pending"; // Pending, Confirmed, Preparing, Ready, Delivered, Cancelled

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal TotalAmount { get; set; }

    public string? Notes { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public List<OrderItem> OrderItems { get; set; } = new();
} 