using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Server.Data;
using RestaurantManagement.Shared.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using RestaurantManagement.Shared.Enums;

namespace RestaurantManagement.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")] // Temporarily comment out for testing
    public class PlatesController : ControllerBase
    {
        private readonly RestaurantDbContext _context;
        private readonly ILogger<PlatesController> _logger;
        private readonly IWebHostEnvironment _environment;

        public PlatesController(
            RestaurantDbContext context, 
            ILogger<PlatesController> logger,
            IWebHostEnvironment environment)
        {
            _context = context;
            _logger = logger;
            _environment = environment;
        }

        // GET: api/Plates
        [HttpGet]
        //[Authorize(Roles = "Admin")] // Temporarily comment out for testing
        public async Task<ActionResult<IEnumerable<Plate>>> GetPlates()
        {
            try
            {
                _logger.LogInformation("Attempting to retrieve plates");
                var plates = await _context.Plates.ToListAsync();
                //log plates
                
                _logger.LogInformation("Retrieved {Count} plates", plates.Count);
                return plates;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving plates");
                return StatusCode(500, "Internal server error while retrieving plates");
            }
        }

        // GET: api/Plates/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Plate>> GetPlate(int id)
        {
            try
            {
                var plate = await _context.Plates.FindAsync(id);

                if (plate == null)
                {
                    _logger.LogWarning("Plate with ID {Id} not found", id);
                    return NotFound($"Plate with ID {id} not found");
                }

                return plate;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving plate with ID {Id}", id);
                return StatusCode(500, "Internal server error while retrieving the plate");
            }
        }

        // PUT: api/Plates/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlate(int id, Plate plate)
        {
            try
            {
                if (id != plate.Id)
                {
                    _logger.LogWarning("Mismatched plate ID: URL ID {UrlId} != Body ID {BodyId}", id, plate.Id);
                    return BadRequest("The ID in the URL does not match the ID in the request body");
                }

                plate.UpdatedAt = DateTime.UtcNow;
                _context.Entry(plate).State = EntityState.Modified;

                await _context.SaveChangesAsync();
                _logger.LogInformation("Successfully updated plate with ID {Id}", id);
                return Ok(plate);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!PlateExists(id))
                {
                    _logger.LogWarning("Plate with ID {Id} not found during update", id);
                    return NotFound($"Plate with ID {id} not found");
                }
                _logger.LogError(ex, "Concurrency error while updating plate with ID {Id}", id);
                return StatusCode(500, "A concurrency error occurred while updating the plate");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating plate with ID {Id}", id);
                return StatusCode(500, "Internal server error while updating the plate");
            }
        }

        // POST: api/Plates/Upload
        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    _logger.LogWarning("No file uploaded or file is empty");
                    return BadRequest("No file uploaded");
                }

                // Validate file size (5MB max)
                if (file.Length > 5 * 1024 * 1024)
                {
                    _logger.LogWarning("File size exceeds 5MB limit");
                    return BadRequest("File size must be less than 5MB");
                }

                // Validate file type
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    _logger.LogWarning("Invalid file type: {FileExtension}", fileExtension);
                    return BadRequest("Only .jpg, .jpeg and .png files are allowed");
                }

                // Ensure wwwroot directory exists
                var wwwrootPath = Path.Combine(_environment.ContentRootPath, "wwwroot");
                if (!Directory.Exists(wwwrootPath))
                {
                    Directory.CreateDirectory(wwwrootPath);
                }

                // Ensure images/plates directory exists
                var imagesPath = Path.Combine(wwwrootPath, "images", "plates");
                if (!Directory.Exists(imagesPath))
                {
                    Directory.CreateDirectory(imagesPath);
                }

                // Generate unique filename
                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var relativePath = Path.Combine("images", "plates", fileName);
                var fullPath = Path.Combine(wwwrootPath, relativePath);

                // Save file
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                _logger.LogInformation("File uploaded successfully: {FileName}", fileName);

                // Return the relative path
                return Ok(new { path = $"/{relativePath.Replace("\\", "/")}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file");
                return StatusCode(500, "Error uploading file");
            }
        }

        // POST: api/Plates
        [HttpPost]
        //[Authorize(Roles = "Admin")] // Temporarily comment out for testing
        public async Task<ActionResult<Plate>> PostPlate([FromBody] Plate plate)
        {
            try
            {
                _logger.LogInformation("Attempting to create new plate: {@Plate}", plate);

                if (!ModelState.IsValid)
                {
                    var errors = string.Join(", ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    _logger.LogWarning("Invalid plate model: {Errors}", errors);
                    return BadRequest(new { Errors = errors });
                }

                // Ensure all required fields are set
                plate.IsAvailable = true; // Force set to true
                plate.CreatedAt = DateTime.UtcNow;
                plate.UpdatedAt = null;

                // Validate Category is a valid enum value
                if (!Enum.IsDefined(typeof(PlateType), plate.Category))
                {
                    _logger.LogWarning("Invalid Category value: {Category}", plate.Category);
                    return BadRequest(new { Error = "Invalid Category value" });
                }

                // Log the plate before saving
                _logger.LogInformation("Plate before saving: {@Plate}", plate);

                _context.Plates.Add(plate);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully created new plate with ID {Id}", plate.Id);
                return CreatedAtAction(nameof(GetPlate), new { id = plate.Id }, plate);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while creating plate");
                return StatusCode(500, new { Error = "Database error while creating the plate", Details = ex.InnerException?.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new plate: {Message}", ex.Message);
                return StatusCode(500, new { Error = "Internal server error while creating the plate", Details = ex.Message });
            }
        }

        // DELETE: api/Plates/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlate(int id)
        {
            try
            {
                var plate = await _context.Plates.FindAsync(id);
                if (plate == null)
                {
                    _logger.LogWarning("Plate with ID {Id} not found for deletion", id);
                    return NotFound($"Plate with ID {id} not found");
                }

                // Delete associated image if it exists
                if (!string.IsNullOrEmpty(plate.ImageUrl))
                {
                    var imagePath = Path.Combine(_environment.WebRootPath, plate.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _context.Plates.Remove(plate);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted plate with ID {Id}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting plate with ID {Id}", id);
                return StatusCode(500, "Internal server error while deleting the plate");
            }
        }

        private bool PlateExists(int id)
        {
            return _context.Plates.Any(e => e.Id == id);
        }
    }
} 