using RestaurantManagement.Shared.Models;
using RestaurantManagement.Shared.Models.Auth;

namespace RestaurantManagement.Client.Auth;

public interface IAuthService
{
    Task<bool> LoginAsync(LoginRequest request);
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task LogoutAsync();
    Task<string> GetTokenAsync();
}
