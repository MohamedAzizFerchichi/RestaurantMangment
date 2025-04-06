using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using RestaurantManagement.Shared.Models;
using RestaurantManagement.Shared.Models.Auth;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;

namespace RestaurantManagement.Client.Auth;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly IJSRuntime _jsRuntime;

    public AuthService(
        HttpClient httpClient,
        AuthenticationStateProvider authStateProvider,
        IJSRuntime jsRuntime)
    {
        _httpClient = httpClient;
        _authStateProvider = authStateProvider;
        _jsRuntime = jsRuntime;
    }

    public async Task<bool> LoginAsync(LoginRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);
        if (response.IsSuccessStatusCode)
        {
            var token = await response.Content.ReadAsStringAsync();
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", token);
            ((CustomAuthStateProvider)_authStateProvider).NotifyUserAuthentication(token);
            return true;
        }
        return false;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/register", request);
        if (response.IsSuccessStatusCode)
        {
            return AuthResponse.CreateSuccess("Registration successful!");
        }
        var errorMessage = await response.Content.ReadAsStringAsync();
        return AuthResponse.CreateFailure(string.IsNullOrEmpty(errorMessage) ? "Registration failed" : errorMessage);
    }

    public async Task LogoutAsync()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
        ((CustomAuthStateProvider)_authStateProvider).NotifyUserLogout();
    }

    public async Task<string> GetTokenAsync()
    {
        return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
    }
}
