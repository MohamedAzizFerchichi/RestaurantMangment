using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace RestaurantManagement.Services;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

    public CustomAuthenticationStateProvider(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");
        var role = await _localStorage.GetItemAsync<string>("userRole");

        if (string.IsNullOrEmpty(token))
            return new AuthenticationState(_anonymous);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, token),
            new Claim(ClaimTypes.Role, role ?? "Client")
        };

        var identity = new ClaimsIdentity(claims, "jwt");
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public void MarkUserAsAuthenticated(string token, string role)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, token),
            new Claim(ClaimTypes.Role, role)
        };

        var identity = new ClaimsIdentity(claims, "jwt");
        var user = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public void MarkUserAsLoggedOut()
    {
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
    }
} 