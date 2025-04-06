using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Data;
using RestaurantManagement.Models;

namespace RestaurantManagement.Services;

public class RestaurantService : IRestaurantService
{
    private readonly RestaurantDbContext _context;

    public RestaurantService(RestaurantDbContext context)
    {
        _context = context;
    }

    // User operations
    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User> CreateUserAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateUserAsync(User user)
    {
        _context.Entry(user).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }

    // Menu operations
    public async Task<List<MenuItem>> GetAllMenuItemsAsync()
    {
        return await _context.MenuItems.ToListAsync();
    }

    public async Task<MenuItem?> GetMenuItemByIdAsync(int id)
    {
        return await _context.MenuItems.FindAsync(id);
    }

    public async Task<MenuItem> CreateMenuItemAsync(MenuItem menuItem)
    {
        _context.MenuItems.Add(menuItem);
        await _context.SaveChangesAsync();
        return menuItem;
    }

    public async Task<MenuItem> UpdateMenuItemAsync(MenuItem menuItem)
    {
        _context.Entry(menuItem).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return menuItem;
    }

    public async Task<bool> DeleteMenuItemAsync(int id)
    {
        var menuItem = await _context.MenuItems.FindAsync(id);
        if (menuItem == null) return false;

        _context.MenuItems.Remove(menuItem);
        await _context.SaveChangesAsync();
        return true;
    }

    // Order operations
    public async Task<List<Order>> GetUserOrdersAsync(int userId)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.MenuItem)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<Order?> GetOrderByIdAsync(int id)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.MenuItem)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<Order> CreateOrderAsync(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<Order> UpdateOrderStatusAsync(int orderId, string status)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order == null)
            throw new ArgumentException("Order not found", nameof(orderId));

        order.Status = status;
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<bool> DeleteOrderAsync(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null) return false;

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
        return true;
    }
} 