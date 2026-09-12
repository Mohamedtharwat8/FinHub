namespace FinHub.Application.Modules.Identity.DTOs;

public sealed record AuthResponse(
    Guid CustomerId,
    string Email,
    string FullName,
    string AccessToken,
    string RefreshToken,
    DateTimeOffset Expiration,
    bool RequiresMfa = false
);
