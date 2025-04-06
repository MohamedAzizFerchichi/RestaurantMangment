using RestaurantManagement.Shared.Models.Auth;

namespace RestaurantManagement.Client.Services;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<bool> ChangePasswordAsync(PasswordChangeRequest request);
    Task LogoutAsync();
    Task<string?> GetTokenAsync();
} 