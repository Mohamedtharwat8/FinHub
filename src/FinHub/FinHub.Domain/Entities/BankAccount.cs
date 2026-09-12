using FinHub.Domain.Enums;
using FinHub.Domain.Events;
using FinHub.Domain.ValueObjects;

namespace FinHub.Domain.Entities;

public sealed class BankAccount
{
    private readonly List<IDomainEvent> _domainEvents = new();
    private readonly List<Transaction> _transactions = new();

    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public IBAN Iban { get; private set; }
    public string AccountNumber { get; private set; }
    public AccountType Type { get; private set; }
    public AccountStatus Status { get; private set; }
    public Money Balance { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    public IReadOnlyCollection<Transaction> Transactions => _transactions.AsReadOnly();

    private BankAccount() 
    { 
        Iban = null!;
        AccountNumber = null!;
        Balance = null!;
    }

    private BankAccount(Guid customerId, IBAN iban, string accountNumber, AccountType type, Money initialBalance)
    {
        Id = Guid.NewGuid();
        CustomerId = customerId;
        Iban = iban ?? throw new ArgumentNullException(nameof(iban));
        AccountNumber = accountNumber;
        Type = type;
        Status = AccountStatus.Active;
        Balance = initialBalance ?? new Money(0, Currency.SAR);
        CreatedAt = DateTimeOffset.UtcNow;

        _domainEvents.Add(new AccountCreatedEvent(Id, CustomerId, Iban.Value, Type));
    }

    public static BankAccount Create(Guid customerId, AccountType type, Money? initialBalance = null)
    {
        if (customerId == Guid.Empty)
            throw new ArgumentException("Customer ID cannot be empty.", nameof(customerId));

        var accNum = Random.Shared.NextInt64(1000000000000000, 9999999999999999).ToString();
        var iban = IBAN.GenerateSaudiIban(accNum);
        var startBal = initialBalance ?? new Money(0, Currency.SAR);

        var account = new BankAccount(customerId, iban, accNum, type, startBal);

        if (startBal.Amount > 0)
        {
            var refNum = "DEP-" + Guid.NewGuid().ToString("N")[..8].ToUpper();
            account._transactions.Add(new Transaction(account.Id, TransactionType.Deposit, startBal, "Initial Account Opening Deposit", refNum));
        }

        return account;
    }

    public void Deposit(Money amount, string description = "Account Deposit")
    {
        EnsureActive();

        if (amount == null || amount.Amount <= 0)
            throw new ArgumentException("Deposit amount must be positive.", nameof(amount));

        Balance = Balance + amount;
        UpdatedAt = DateTimeOffset.UtcNow;

        var refNum = "DEP-" + Guid.NewGuid().ToString("N")[..8].ToUpper();
        _transactions.Add(new Transaction(Id, TransactionType.Deposit, amount, description, refNum));
        _domainEvents.Add(new MoneyDepositedEvent(Id, amount, refNum));
    }

    public void Withdraw(Money amount, string description = "Account Withdrawal")
    {
        EnsureActive();

        if (amount == null || amount.Amount <= 0)
            throw new ArgumentException("Withdrawal amount must be positive.", nameof(amount));

        if (Balance.Amount < amount.Amount)
            throw new InvalidOperationException($"Insufficient funds. Current balance: {Balance.Amount} {Balance.Currency}. Required: {amount.Amount} {amount.Currency}.");

        Balance = Balance - amount;
        UpdatedAt = DateTimeOffset.UtcNow;

        var refNum = "WTH-" + Guid.NewGuid().ToString("N")[..8].ToUpper();
        _transactions.Add(new Transaction(Id, TransactionType.Withdrawal, amount, description, refNum));
        _domainEvents.Add(new MoneyWithdrawnEvent(Id, amount, refNum));
    }

    public void Freeze(string reason = "Security Hold")
    {
        Status = AccountStatus.Frozen;
        UpdatedAt = DateTimeOffset.UtcNow;
        _domainEvents.Add(new AccountFrozenEvent(Id, reason));
    }

    public void Unfreeze()
    {
        Status = AccountStatus.Active;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void ClearDomainEvents() => _domainEvents.Clear();

    private void EnsureActive()
    {
        if (Status != AccountStatus.Active)
            throw new InvalidOperationException($"Account operation failed. Account status is currently '{Status}'.");
    }
}
