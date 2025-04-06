using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using RestaurantManagement.Server.Data;
using RestaurantManagement.Shared.Models;
using RestaurantManagement.Shared.Models.Auth;
using BCrypt.Net;

namespace RestaurantManagement.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IConfiguration _configuration;
    private static readonly DateTime _defaultDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        if (await _userManager.FindByEmailAsync(request.Email) != null)
        {
            return BadRequest(AuthResponse.CreateFailure("Email already exists"));
        }

        var user = new User
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Role = "User",
            CreatedAt = _defaultDate
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return BadRequest(AuthResponse.CreateFailure(string.Join(", ", result.Errors.Select(e => e.Description))));
        }

        await _userManager.AddToRoleAsync(user, "User");
        var token = GenerateJwtToken(user, "User");
        
        return Ok(new AuthResponse 
        { 
            Success = true, 
            Message = "Registration successful",
            Token = token,
            User = new UserDto 
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = "User"
            }
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        //log the request
        Console.WriteLine($"Login request: {request.Email} {request.Password}");
        if (user == null)
        {
            return Unauthorized(AuthResponse.CreateFailure("Invalid email or password"));
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        //log the result
        Console.WriteLine($"Login result: {result}");
        if (!result.Succeeded)
        {
            if (result.IsLockedOut)
            {
                return Unauthorized(AuthResponse.CreateFailure("Account is locked. Please try again later."));
            }
            return Unauthorized(AuthResponse.CreateFailure("Invalid email or password"));
        }

        // Get user roles
        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? user.Role;

        var token = GenerateJwtToken(user, role);
        user.LastLoginAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        return Ok(new AuthResponse 
        { 
            Success = true,
            Message = "Login successful",
            Token = token,
            User = new UserDto 
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = role
            }
        });
    }

    private string GenerateJwtToken(User user, string role)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"] ?? throw new InvalidOperationException("JWT Key not found")));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.GivenName, user.FirstName),
            new Claim(ClaimTypes.Surname, user.LastName),
            new Claim(ClaimTypes.Role, role)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JwtSettings:ExpiryInMinutes"])),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
} 