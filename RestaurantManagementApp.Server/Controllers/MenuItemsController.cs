using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantManagementApp.Server.Data;
using RestaurantManagementApp.Shared.Models;

namespace RestaurantManagementApp.Server.Controllers;

public class MenuItemsController : BaseApiController
{
    public MenuItemsController(RestaurantDbContext context, ILogger<MenuItemsController> logger)
        : base(context, logger)
    {
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MenuItem>>> GetMenuItems()
    {
        try
        {
            var menuItems = await _context.MenuItems.ToListAsync();
            return Ok(menuItems);
        }
        catch (Exception ex)
        {
            return HandleException<IEnumerable<MenuItem>>(ex);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MenuItem>> GetMenuItem(int id)
    {
        try
        {
            var menuItem = await _context.MenuItems.FindAsync(id);

            if (menuItem == null)
            {
                return NotFound();
            }

            return Ok(menuItem);
        }
        catch (Exception ex)
        {
            return HandleException<MenuItem>(ex);
        }
    }

    [HttpPost]
    public async Task<ActionResult<MenuItem>> CreateMenuItem(MenuItem menuItem)
    {
        try
        {
            _context.MenuItems.Add(menuItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMenuItem), new { id = menuItem.Id }, menuItem);
        }
        catch (Exception ex)
        {
            return HandleException<MenuItem>(ex);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMenuItem(int id, MenuItem menuItem)
    {
        try
        {
            if (id != menuItem.Id)
            {
                return BadRequest();
            }

            menuItem.UpdatedAt = DateTime.UtcNow;
            _context.Entry(menuItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await MenuItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleException<IActionResult>(ex);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMenuItem(int id)
    {
        try
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
            {
                return NotFound();
            }

            _context.MenuItems.Remove(menuItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleException<IActionResult>(ex);
        }
    }

    private async Task<bool> MenuItemExists(int id)
    {
        return await _context.MenuItems.AnyAsync(e => e.Id == id);
    }
} 