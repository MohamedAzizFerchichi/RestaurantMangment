using Microsoft.AspNetCore.Mvc;
using RestaurantManagementApp.Server.Data;

namespace RestaurantManagementApp.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    protected readonly RestaurantDbContext _context;
    protected readonly ILogger<BaseApiController> _logger;

    protected BaseApiController(RestaurantDbContext context, ILogger<BaseApiController> logger)
    {
        _context = context;
        _logger = logger;
    }

    protected ActionResult<T> HandleException<T>(Exception ex)
    {
        _logger.LogError(ex, "An error occurred while processing the request");
        return StatusCode(500, new { error = "An internal server error occurred" });
    }
} 