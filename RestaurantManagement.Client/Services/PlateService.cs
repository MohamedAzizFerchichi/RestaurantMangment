using System.Net.Http.Json;
using RestaurantManagement.Shared.Models;

namespace RestaurantManagement.Client.Services;

public class PlateService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "api/plates";

    public PlateService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Plate>> GetPlatesAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<Plate>>(BaseUrl) ?? new List<Plate>();
    }

    public async Task<Plate> GetPlateAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<Plate>($"{BaseUrl}/{id}");
    }

    public async Task<Plate> CreatePlateAsync(Plate plate)
    {
        var response = await _httpClient.PostAsJsonAsync(BaseUrl, plate);
        return await response.Content.ReadFromJsonAsync<Plate>();
    }

    public async Task UpdatePlateAsync(int id, Plate plate)
    {
        await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", plate);
    }

    public async Task DeletePlateAsync(int id)
    {
        await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
    }

    public async Task<string> UploadPlateImageAsync(MultipartFormDataContent content)
    {
        var response = await _httpClient.PostAsync($"{BaseUrl}/upload", content);
        return await response.Content.ReadAsStringAsync();
    }
} 