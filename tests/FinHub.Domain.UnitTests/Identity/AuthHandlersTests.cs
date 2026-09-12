using FinHub.Application.Modules.Identity.DTOs;
using FinHub.Application.Modules.Identity.Services;
using FinHub.Infrastructure.Persistence;
using FinHub.Infrastructure.Security;

namespace FinHub.Domain.UnitTests.Identity;

public class AuthHandlersTests
{
    private readonly IdentityService _service;

    public AuthHandlersTests()
    {
        var repo = new CustomerRepository();
        var hasher = new PasswordHasher();
        var jwt = new JwtTokenGenerator();
        var totp = new TotpService();
        var otp = new OtpService();
        var oauth = new OAuthService();
        _service = new IdentityService(repo, hasher, jwt, totp, otp, oauth);
    }

    [Fact]
    public async Task Register_ValidRequest_ShouldReturnAuthResponseWithTokens()
    {
        // Arrange
        var req = new RegisterRequest("tariq@banking.sa", "Tariq Mansoor", "SecurePassword123!");

        // Act
        var res = await _service.RegisterAsync(req);

        // Assert
        Assert.NotEqual(Guid.Empty, res.CustomerId);
        Assert.Equal("tariq@banking.sa", res.Email);
        Assert.False(string.IsNullOrWhiteSpace(res.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(res.RefreshToken));
    }

    [Fact]
    public async Task Login_ValidCredentials_ShouldSucceed()
    {
        // Arrange
        var reg = new RegisterRequest("nasser@banking.sa", "Nasser Al-Otaibi", "Password456!");
        await _service.RegisterAsync(reg);

        // Act
        var loginRes = await _service.LoginAsync(new LoginRequest("nasser@banking.sa", "Password456!"));

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(loginRes.AccessToken));
    }

    [Fact]
    public async Task RefreshToken_ValidToken_ShouldRotateRefreshToken()
    {
        // Arrange
        var reg = new RegisterRequest("reem@banking.sa", "Reem Saad", "Password789!");
        var regRes = await _service.RegisterAsync(reg);

        // Act
        var refreshRes = await _service.RefreshTokenAsync(new RefreshTokenRequest(regRes.RefreshToken));

        // Assert
        Assert.NotEqual(regRes.RefreshToken, refreshRes.RefreshToken);
        Assert.False(string.IsNullOrWhiteSpace(refreshRes.AccessToken));
    }
}
