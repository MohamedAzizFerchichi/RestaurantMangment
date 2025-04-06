namespace RestaurantManagement.Shared.Models.Auth;

public class AuthResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? Token { get; set; } // Added Token property
    public UserDto? User { get; set; }

    public static AuthResponse CreateSuccess(string message = "Operation successful")
    {
        return new AuthResponse { Success = true, Message = message };
    }

    public static AuthResponse CreateFailure(string message = "Operation failed")
    {
        return new AuthResponse { Success = false, Message = message };
    }
}
