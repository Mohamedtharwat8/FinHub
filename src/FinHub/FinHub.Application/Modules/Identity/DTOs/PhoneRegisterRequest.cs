namespace FinHub.Application.Modules.Identity.DTOs;

public sealed record PhoneRegisterRequest(
    string PhoneNumber,
    string FullName,
    string OtpCode
);
