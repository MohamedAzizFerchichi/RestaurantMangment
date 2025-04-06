using System.Net.Http.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization; // Added this line
using RestaurantManagement.Shared.Models.Auth;
using RestaurantManagement.Client.Auth;

namespace RestaurantManagement.Client.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private readonly AuthenticationStateProvider _authStateProvider;

    public AuthService(
        HttpClient httpClient,
        ILocalStorageService localStorage,
        AuthenticationStateProvider authStateProvider)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
        _authStateProvider = authStateProvider;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
            if (result?.Token != null)
            {
                await _localStorage.SetItemAsync("authToken", result.Token);
                ((CustomAuthStateProvider)_authStateProvider).NotifyUserAuthentication(result.Token);
            }
            return result!;
        }
        throw new Exception(await response.Content.ReadAsStringAsync());
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/register", request);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
            if (result?.Token != null)
            {
                await _localStorage.SetItemAsync("authToken", result.Token);
                ((CustomAuthStateProvider)_authStateProvider).NotifyUserAuthentication(result.Token);
            }
            return result!;
        }
        throw new Exception(await response.Content.ReadAsStringAsync());
    }

    public async Task<bool> ChangePasswordAsync(PasswordChangeRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/change-password", request);
        return response.IsSuccessStatusCode;
    }

    public async Task LogoutAsync()
    {
        await _localStorage.RemoveItemAsync("authToken");
        ((CustomAuthStateProvider)_authStateProvider).NotifyUserLogout();
    }

    public async Task<string?> GetTokenAsync()
    {
        return await _localStorage.GetItemAsync<string>("authToken");
    }
}
