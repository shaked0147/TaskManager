using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManagerApi.Models;

namespace TaskManagerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly JwtSettings _jwtSettings;

    private readonly Dictionary<string, string> _users = new()
        {
            { "admin", "password123" },
            { "test", "test123" }
        };

    public UserController(ILogger<UserController> logger, IOptions<JwtSettings> jwtOptions)
    {
        _logger = logger;
        _jwtSettings = jwtOptions.Value;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] User request)
    {
        try
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { message = "Username and password are required." });
            }

            var isValidUser = await Task.Run(() =>
                _users.ContainsKey(request.Username) && _users[request.Username] == request.Password
            );

            if (!isValidUser)
            {
                _logger.LogWarning("Failed login attempt for user {Username}", request.Username);
                return Unauthorized(new { message = "Invalid username or password." });
            }

            var token = GenerateJwtToken(request.Username);

            _logger.LogInformation("User {Username} logged in successfully at {Time}", request.Username, DateTime.UtcNow);

            return Ok(new { token });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during login for user {Username}", request?.Username);
            return StatusCode(500, new { message = "Internal server error. Please try again later." });
        }
    }

    private string GenerateJwtToken(string username)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSettings.Secret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
