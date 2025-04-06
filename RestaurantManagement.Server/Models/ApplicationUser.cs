using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace RestaurantManagement.Server.Models;

public class ApplicationUser : IdentityUser
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
} 