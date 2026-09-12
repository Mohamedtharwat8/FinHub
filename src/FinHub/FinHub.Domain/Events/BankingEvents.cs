using FinHub.Domain.Enums;
using FinHub.Domain.ValueObjects;

namespace FinHub.Domain.Events;

public sealed record AccountCreatedEvent(Guid AccountId, Guid CustomerId, string Iban, AccountType Type) : IDomainEvent;
public sealed record MoneyDepositedEvent(Guid AccountId, Money Amount, string ReferenceNumber) : IDomainEvent;
public sealed record MoneyWithdrawnEvent(Guid AccountId, Money Amount, string ReferenceNumber) : IDomainEvent;
public sealed record AccountFrozenEvent(Guid AccountId, string Reason) : IDomainEvent;
