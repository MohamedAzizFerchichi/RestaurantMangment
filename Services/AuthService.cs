using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using RestaurantManagement.Models;

namespace RestaurantManagement.Services;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task LogoutAsync();
    Task<bool> IsAuthenticatedAsync();
    Task<string?> GetUserRoleAsync();
}

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;
    private readonly AuthenticationStateProvider _authStateProvider;

    public AuthService(
        HttpClient httpClient,
        IJSRuntime jsRuntime,
        AuthenticationStateProvider authStateProvider)
    {
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
        _authStateProvider = authStateProvider;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);
        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        
        if (result?.Success == true && !string.IsNullOrEmpty(result.Token))
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", result.Token);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "userRole", result.Role);
            (_authStateProvider as CustomAuthenticationStateProvider)?.MarkUserAsAuthenticated(result.Token, result.Role);
        }
        
        return result ?? new AuthResponse { Success = false, Message = "Login failed" };
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/register", request);
        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        
        if (result?.Success == true && !string.IsNullOrEmpty(result.Token))
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", result.Token);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "userRole", result.Role);
            (_authStateProvider as CustomAuthenticationStateProvider)?.MarkUserAsAuthenticated(result.Token, result.Role);
        }
        
        return result ?? new AuthResponse { Success = false, Message = "Registration failed" };
    }

    public async Task LogoutAsync()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "userRole");
        (_authStateProvider as CustomAuthenticationStateProvider)?.MarkUserAsLoggedOut();
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
        return !string.IsNullOrEmpty(token);
    }

    public async Task<string?> GetUserRoleAsync()
    {
        return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "userRole");
    }
} 