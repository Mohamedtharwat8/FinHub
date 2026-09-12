namespace FinHub.Application.Modules.Customer.DTOs;

public sealed record CustomerProfileDto(
    Guid Id,
    string Email,
    string FullName,
    string Role,
    string? NationalId,
    bool IsCitizen,
    AddressDto? Address,
    bool IsEmailVerified,
    bool IsMfaEnabled,
    DateTimeOffset CreatedAt
);

public sealed record AddressDto(
    string BuildingNumber,
    string Street,
    string District,
    string City,
    string PostalCode,
    string? AdditionalNumber,
    string Country
);
