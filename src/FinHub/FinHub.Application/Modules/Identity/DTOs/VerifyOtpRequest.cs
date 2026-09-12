namespace FinHub.Application.Modules.Identity.DTOs;

public sealed record VerifyOtpRequest(
    string Target,
    string Code,
    string Purpose = "Registration"
);
