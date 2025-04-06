using System.ComponentModel.DataAnnotations;

namespace RestaurantManagementApp.Shared.Models;

public class Order
{
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    [Required]
    public string Status { get; set; } = "Pending";

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal TotalAmount { get; set; }

    public string? Notes { get; set; }

    public User User { get; set; } = null!;
    public List<OrderItem> OrderItems { get; set; } = new();
} 