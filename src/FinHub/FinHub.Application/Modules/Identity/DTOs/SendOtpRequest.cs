namespace FinHub.Application.Modules.Identity.DTOs;

public sealed record SendOtpRequest(
    string Target,
    string Purpose = "Registration"
);
