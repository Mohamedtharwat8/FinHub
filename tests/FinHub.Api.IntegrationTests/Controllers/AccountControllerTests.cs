using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FinHub.Application.Modules.Banking.DTOs;
using FinHub.Application.Modules.Identity.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit.Abstractions;

namespace FinHub.Api.IntegrationTests.Controllers;

public class AccountControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public AccountControllerTests(WebApplicationFactory<Program> factory, ITestOutputHelper output)
    {
        _client = factory.CreateClient();
        _output = output;
    }

    [Fact]
    public async Task CreateAccount_Deposit_And_Withdraw_ShouldWorkEndToEnd()
    {
        // 1. Register customer
        var regReq = new RegisterRequest("banktest@finhub.sa", "Bank Test User", "StrongPassword123!");
        var regRes = await _client.PostAsJsonAsync("/api/v1/auth/register", regReq);
        var authData = await regRes.Content.ReadFromJsonAsync<AuthResponse>(_jsonOptions);
        Assert.NotNull(authData);

        // 2. Open Savings Account with 1000 SAR
        var createCmd = new CreateAccountCommand(authData.CustomerId, "Savings", 1000, "SAR");
        var createRes = await _client.PostAsJsonAsync("/api/v1/accounts", createCmd);
        var createStr = await createRes.Content.ReadAsStringAsync();
        _output.WriteLine($"CREATE RESPONSE JSON: {createStr}");
        Assert.Equal(HttpStatusCode.Created, createRes.StatusCode);

        var acc = await createRes.Content.ReadFromJsonAsync<BankAccountDto>(_jsonOptions);
        Assert.NotNull(acc);
        _output.WriteLine($"PARSED ACC ID: {acc.Id}");
        Assert.NotEqual(Guid.Empty, acc.Id);

        // 3. Deposit 500 SAR
        var depRes = await _client.PostAsJsonAsync($"/api/v1/accounts/{acc.Id}/deposit", new DepositCommand(500, "SAR", "Salary Transfer"));
        var depStr = await depRes.Content.ReadAsStringAsync();
        _output.WriteLine($"DEPOSIT RESPONSE JSON: {depStr}");
        Assert.Equal(HttpStatusCode.OK, depRes.StatusCode);
    }
}
