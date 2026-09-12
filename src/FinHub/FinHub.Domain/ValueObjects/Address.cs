namespace FinHub.Domain.ValueObjects;

public sealed record Address
{
    public string BuildingNumber { get; }
    public string Street { get; }
    public string District { get; }
    public string City { get; }
    public string PostalCode { get; }
    public string? AdditionalNumber { get; }
    public string Country { get; }

    public Address(
        string buildingNumber,
        string street,
        string district,
        string city,
        string postalCode,
        string? additionalNumber = null,
        string country = "Saudi Arabia")
    {
        if (string.IsNullOrWhiteSpace(buildingNumber)) throw new ArgumentException("Building number is required.", nameof(buildingNumber));
        if (string.IsNullOrWhiteSpace(street)) throw new ArgumentException("Street is required.", nameof(street));
        if (string.IsNullOrWhiteSpace(district)) throw new ArgumentException("District is required.", nameof(district));
        if (string.IsNullOrWhiteSpace(city)) throw new ArgumentException("City is required.", nameof(city));
        if (string.IsNullOrWhiteSpace(postalCode)) throw new ArgumentException("Postal code is required.", nameof(postalCode));

        BuildingNumber = buildingNumber.Trim();
        Street = street.Trim();
        District = district.Trim();
        City = city.Trim();
        PostalCode = postalCode.Trim();
        AdditionalNumber = additionalNumber?.Trim();
        Country = country.Trim();
    }
}
