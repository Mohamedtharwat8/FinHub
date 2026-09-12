namespace FinHub.Application.Modules.Identity.DTOs;

public sealed record RegisterRequest(
    string Email,
    string FullName,
    string Password
);
