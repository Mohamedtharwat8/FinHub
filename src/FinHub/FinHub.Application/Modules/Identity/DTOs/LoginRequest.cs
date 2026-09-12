namespace FinHub.Application.Modules.Identity.DTOs;

public sealed record LoginRequest(
    string Email,
    string Password,
    string? TotpCode = null
);
