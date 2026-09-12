using System.Net;
using System.Net.Http.Json;
using FinHub.Application.Modules.Identity.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;

namespace FinHub.Api.IntegrationTests.Controllers;

public class AuthControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AuthControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_ValidPayload_ShouldReturn200OKWithJwtToken()
    {
        // Arrange
        var request = new RegisterRequest("fahad@finhub.sa", "Fahad Al-Sudairi", "StrongPassword123!");

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var authRes = await response.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(authRes);
        Assert.Equal("fahad@finhub.sa", authRes.Email);
        Assert.False(string.IsNullOrWhiteSpace(authRes.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(authRes.RefreshToken));
    }

    [Fact]
    public async Task Login_InvalidPassword_ShouldReturn401Unauthorized()
    {
        // Arrange
        var regRequest = new RegisterRequest("badlogin@finhub.sa", "User Test", "Password123!");
        await _client.PostAsJsonAsync("/api/v1/auth/register", regRequest);

        var loginRequest = new LoginRequest("badlogin@finhub.sa", "WrongPassword!");

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
