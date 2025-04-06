using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace RestaurantManagement.Shared.Models;

public class User : IdentityUser
{
    [Required]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = "User";

    public DateTime CreatedAt { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public List<Order> Orders { get; set; } = new();
} 