using FinHub.Domain.Enums;
using FinHub.Domain.ValueObjects;

namespace FinHub.Domain.Entities;

public sealed class Transaction
{
    public Guid Id { get; private set; }
    public Guid AccountId { get; private set; }
    public TransactionType Type { get; private set; }
    
    public decimal AmountValue { get; private set; }
    public Currency Currency { get; private set; }

    public Money Amount
    {
        get => new(AmountValue, Currency);
        private set
        {
            AmountValue = value.Amount;
            Currency = value.Currency;
        }
    }

    public string Description { get; private set; }
    public string ReferenceNumber { get; private set; }
    public DateTimeOffset TransactionDate { get; private set; }

    private Transaction() 
    {
        Description = null!;
        ReferenceNumber = null!;
    }

    public Transaction(Guid accountId, TransactionType type, Money amount, string description, string referenceNumber)
    {
        Id = Guid.Empty; // Allow EF Core to identify newly added entities
        AccountId = accountId;
        Type = type;
        Amount = amount ?? throw new ArgumentNullException(nameof(amount));
        Description = description ?? "Bank Transaction";
        ReferenceNumber = referenceNumber ?? Guid.NewGuid().ToString("N")[..12].ToUpper();
        TransactionDate = DateTimeOffset.UtcNow;
    }
}
