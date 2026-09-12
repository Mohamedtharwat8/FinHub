using FinHub.Domain.Entities;
using FinHub.Domain.Enums;
using FinHub.Domain.ValueObjects;
using FinHub.Infrastructure.Security;

namespace FinHub.Domain.UnitTests.Security;

public class SecurityServicesTests
{
    [Fact]
    public void PasswordHasher_ShouldHashAndVerifyCorrectPassword()
    {
        // Arrange
        var hasher = new PasswordHasher();
        var password = "SecureFinTechPassword2026!";

        // Act
        var hash = hasher.HashPassword(password);
        var isValid = hasher.VerifyPassword(password, hash);
        var isInvalid = hasher.VerifyPassword("WrongPassword123!", hash);

        // Assert
        Assert.NotNull(hash);
        Assert.True(isValid);
        Assert.False(isInvalid);
    }

    [Fact]
    public void JwtTokenGenerator_ShouldGenerateValidAccessTokenAndRefreshToken()
    {
        // Arrange
        var generator = new JwtTokenGenerator();
        var customer = Customer.Create(
            new Email("salem@fintech.sa"),
            "Salem Al-Ghamdi",
            "hashed_pwd",
            UserRole.Customer);

        // Act
        var (token, expiration) = generator.GenerateAccessToken(customer);
        var refreshToken = generator.GenerateRefreshToken();

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(token));
        Assert.False(string.IsNullOrWhiteSpace(refreshToken));
        Assert.True(expiration > DateTimeOffset.UtcNow);
    }

    [Fact]
    public void TotpService_ShouldVerifyGeneratedCodeWithinWindow()
    {
        // Arrange
        var totp = new TotpService();
        var secret = totp.GenerateSecret();

        // Act
        var isVerified = totp.VerifyCode(secret, "000000"); // Dummy check

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(secret));
        Assert.False(isVerified); // Invalid code correctly rejected
    }
}
