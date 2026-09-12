using FinHub.Application.Modules.Identity.DTOs;
using FinHub.Application.Modules.Identity.Services;
using FinHub.Infrastructure.Persistence;
using FinHub.Infrastructure.Security;

namespace FinHub.Domain.UnitTests.Identity;

public class OAuthAndOtpAuthTests
{
    private readonly IdentityService _service;

    public OAuthAndOtpAuthTests()
    {
        var repo = new CustomerRepository();
        var hasher = new PasswordHasher();
        var jwt = new JwtTokenGenerator();
        var totp = new TotpService();
        var otp = new OtpService();
        var oauth = new OAuthService();
        _service = new IdentityService(repo, hasher, jwt, totp, otp, oauth);
    }

    [Theory]
    [InlineData("Google")]
    [InlineData("GitHub")]
    [InlineData("Microsoft")]
    public async Task ExternalLogin_ValidOAuthProviders_ShouldAuthenticateAndReturnJwt(string provider)
    {
        // Arrange
        var req = new ExternalLoginRequest(provider, "oauth-token-12345");

        // Act
        var res = await _service.ExternalLoginAsync(req);

        // Assert
        Assert.NotNull(res);
        Assert.False(string.IsNullOrWhiteSpace(res.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(res.RefreshToken));
        Assert.NotEqual(Guid.Empty, res.CustomerId);
    }

    [Fact]
    public async Task SendAndVerifyOtp_ValidCode_ShouldSucceed()
    {
        // 1. Send OTP
        var target = "0501234567";
        var otpCode = await _service.SendOtpAsync(new SendOtpRequest(target, "Registration"));

        Assert.Equal(6, otpCode.Length);

        // 2. Verify OTP
        var isValid = await _service.VerifyOtpAsync(new VerifyOtpRequest(target, otpCode, "Registration"));
        Assert.True(isValid);
    }

    [Fact]
    public async Task RegisterWithPhone_ValidOtp_ShouldCreateCustomerAccount()
    {
        // 1. Send OTP
        var phone = "+966512345678";
        var otpCode = await _service.SendOtpAsync(new SendOtpRequest(phone, "PhoneRegistration"));

        // 2. Register
        var req = new PhoneRegisterRequest(phone, "Saad Al-Ghamdi", otpCode);
        var res = await _service.RegisterWithPhoneAsync(req);

        // Assert
        Assert.NotNull(res);
        Assert.Equal("Saad Al-Ghamdi", res.FullName);
        Assert.False(string.IsNullOrWhiteSpace(res.AccessToken));
    }
}
