using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FinHub.Application.Common.Interfaces;
using FinHub.Domain.Entities;
using Microsoft.IdentityModel.Tokens;

namespace FinHub.Infrastructure.Security;

public sealed class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly TimeSpan _tokenLifetime;

    public JwtTokenGenerator(
        string secretKey = "FinHub_SuperSecret_SAMA_Security_Key_2026_DotNet10!",
        string issuer = "FinHub.Api",
        string audience = "FinHub.Client",
        TimeSpan? tokenLifetime = null)
    {
        _secretKey = secretKey;
        _issuer = issuer;
        _audience = audience;
        _tokenLifetime = tokenLifetime ?? TimeSpan.FromHours(1);
    }

    public (string Token, DateTimeOffset Expiration) GenerateAccessToken(Customer customer)
    {
        var expiration = DateTimeOffset.UtcNow.Add(_tokenLifetime);
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_secretKey);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, customer.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, customer.Email.Value),
            new(JwtRegisteredClaimNames.Name, customer.FullName),
            new(ClaimTypes.Role, customer.Role.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiration.UtcDateTime,
            Issuer = _issuer,
            Audience = _audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return (tokenHandler.WriteToken(token), expiration);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
