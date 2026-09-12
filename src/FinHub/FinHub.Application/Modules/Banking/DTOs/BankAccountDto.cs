namespace FinHub.Application.Modules.Banking.DTOs;

public sealed class BankAccountDto
{
    public Guid Id { get; init; }
    public Guid CustomerId { get; init; }
    public string Iban { get; init; } = string.Empty;
    public string AccountNumber { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public decimal BalanceAmount { get; init; }
    public string Currency { get; init; } = "SAR";
    public DateTimeOffset CreatedAt { get; init; }
}

public sealed class TransactionDto
{
    public Guid Id { get; init; }
    public Guid AccountId { get; init; }
    public string Type { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "SAR";
    public string Description { get; init; } = string.Empty;
    public string ReferenceNumber { get; init; } = string.Empty;
    public DateTimeOffset TransactionDate { get; init; }
}
