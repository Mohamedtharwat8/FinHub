namespace FinHub.Application.Modules.Identity.DTOs;

public sealed record ExternalLoginRequest(
    string Provider,
    string Token
);
