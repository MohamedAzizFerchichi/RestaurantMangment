using Blazored.LocalStorage;

namespace RestaurantManagement.Client.Services;

public class LocalStorageService : ILocalStorageService
{
    private readonly ILocalStorageService _localStorage;

    public LocalStorageService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task<T?> GetItemAsync<T>(string key)
    {
        try
        {
            return await _localStorage.GetItemAsync<T>(key);
        }
        catch
        {
            return default;
        }
    }

    public async Task SetItemAsync<T>(string key, T value)
    {
        try
        {
            await _localStorage.SetItemAsync(key, value);
        }
        catch
        {
            // Log error or handle appropriately
        }
    }

    public async Task RemoveItemAsync(string key)
    {
        try
        {
            await _localStorage.RemoveItemAsync(key);
        }
        catch
        {
            // Log error or handle appropriately
        }
    }
} 