namespace FinHub.Application.Modules.Customer.DTOs;

public sealed record UpdateCustomerAddressCommand(
    string BuildingNumber,
    string Street,
    string District,
    string City,
    string PostalCode,
    string? AdditionalNumber = null,
    string Country = "Saudi Arabia"
);
