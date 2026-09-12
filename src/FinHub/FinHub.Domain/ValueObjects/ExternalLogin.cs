namespace FinHub.Domain.ValueObjects;

public sealed record ExternalLogin(
    string Provider,
    string ProviderKey,
    string Email,
    DateTimeOffset LinkedAt
);
