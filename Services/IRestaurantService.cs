using RestaurantManagement.Models;

namespace RestaurantManagement.Services;

public interface IRestaurantService
{
    // User operations
    Task<User?> GetUserByIdAsync(int id);
    Task<User?> GetUserByEmailAsync(string email);
    Task<User> CreateUserAsync(User user);
    Task<User> UpdateUserAsync(User user);
    Task<bool> DeleteUserAsync(int id);

    // Menu operations
    Task<List<MenuItem>> GetAllMenuItemsAsync();
    Task<MenuItem?> GetMenuItemByIdAsync(int id);
    Task<MenuItem> CreateMenuItemAsync(MenuItem menuItem);
    Task<MenuItem> UpdateMenuItemAsync(MenuItem menuItem);
    Task<bool> DeleteMenuItemAsync(int id);

    // Order operations
    Task<List<Order>> GetUserOrdersAsync(int userId);
    Task<Order?> GetOrderByIdAsync(int id);
    Task<Order> CreateOrderAsync(Order order);
    Task<Order> UpdateOrderStatusAsync(int orderId, string status);
    Task<bool> DeleteOrderAsync(int id);
} 