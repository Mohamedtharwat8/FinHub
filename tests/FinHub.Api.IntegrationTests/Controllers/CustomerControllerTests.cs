using System.Net;
using System.Net.Http.Json;
using FinHub.Application.Modules.Customer.DTOs;
using FinHub.Application.Modules.Identity.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;

namespace FinHub.Api.IntegrationTests.Controllers;

public class CustomerControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public CustomerControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetProfile_ExistingCustomer_ShouldReturn200OKWithProfile()
    {
        // 1. Register customer
        var regReq = new RegisterRequest("profiletest@finhub.sa", "Profile Test User", "StrongPassword123!");
        var regRes = await _client.PostAsJsonAsync("/api/v1/auth/register", regReq);
        var authData = await regRes.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(authData);

        // 2. Query Profile
        var response = await _client.GetAsync($"/api/v1/customer/{authData.CustomerId}/profile");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var profile = await response.Content.ReadFromJsonAsync<CustomerProfileDto>();
        Assert.NotNull(profile);
        Assert.Equal("profiletest@finhub.sa", profile.Email);
        Assert.Equal("Profile Test User", profile.FullName);
    }
}
