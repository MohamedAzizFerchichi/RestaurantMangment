using System.Net.Http.Json;
using RestaurantManagement.Shared.Models;


using RestaurantManagement.Client.Services;
using RestaurantManagement.Shared.Models;


public class OrderService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "api/orders";

    public OrderService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Order>> GetOrdersAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<Order>>(BaseUrl) ?? new List<Order>();
    }

    public async Task<Order> GetOrderAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<Order>($"{BaseUrl}/{id}");
    }

    public async Task UpdateOrderStatusAsync(int id, OrderStatus status)
    {
        await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}/status", status);
    }

    public async Task DeleteOrderAsync(int id)
    {
        await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
    }
}
