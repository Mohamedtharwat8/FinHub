using FinHub.Domain.ValueObjects;

namespace FinHub.Domain.Events;

public sealed record CustomerRegisteredEvent(
    Guid CustomerId,
    Email Email,
    string FullName,
    DateTimeOffset OccurredOn
);
