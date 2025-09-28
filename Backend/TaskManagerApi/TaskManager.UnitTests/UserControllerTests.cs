using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Text.Json;
using TaskManagerApi.Controllers;
using TaskManagerApi.Models;
using Xunit;

namespace TaskManager.UnitTests;

public class UserControllerTests
{
    private readonly Mock<ILogger<UserController>> _loggerMock = new();
    private readonly Mock<IConfiguration> _configMock = new();

    private UserController CreateController()
    {
        var jwtSettings = new JwtSettings
        {
            Secret = "TestSecretKey123123123123TestSecretKeyTestSecretKey123123123123TestSecretKey",
            ExpiryHours = 1
        };
        var optionsMock = Mock.Of<IOptions<JwtSettings>>(o => o.Value == jwtSettings);

        return new UserController(_loggerMock.Object, optionsMock);
    }

    [Fact]
    public async Task Login_ValidUser_ReturnsToken()
    {
        var controller = CreateController();
        var request = new User { Username = "admin", Password = "password123" };

        var result = await controller.Login(request);

        var okResult = Assert.IsType<OkObjectResult>(result);

        // Serialize & deserialize as JsonElement
        var json = JsonSerializer.Serialize(okResult.Value);
        var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        // Access "token" property
        Assert.True(root.TryGetProperty("token", out var tokenProperty));
        Assert.False(string.IsNullOrEmpty(tokenProperty.GetString()));
    }

    [Fact]
    public async Task Login_InvalidUser_ReturnsUnauthorized()
    {
        var controller = CreateController();
        var request = new User { Username = "admin", Password = "wrongpassword" };

        var result = await controller.Login(request);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task Login_MissingUsername_ReturnsBadRequest()
    {
        var controller = CreateController();
        var request = new User { Username = "", Password = "password123" };

        var result = await controller.Login(request);

        Assert.IsType<BadRequestObjectResult>(result);
    }
}