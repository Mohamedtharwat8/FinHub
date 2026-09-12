using FinHub.Domain.Entities;

namespace FinHub.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    (string Token, DateTimeOffset Expiration) GenerateAccessToken(Customer customer);
    string GenerateRefreshToken();
}
