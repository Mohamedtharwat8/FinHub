using FinHub.Domain.Entities;
using FinHub.Domain.Enums;
using FinHub.Domain.ValueObjects;

namespace FinHub.Domain.UnitTests.Entities;

public class BankAccountTests
{
    [Fact]
    public void Create_ValidParameters_ShouldInstantiateSaudiIbanAndPublishEvent()
    {
        // Act
        var customerId = Guid.NewGuid();
        var account = BankAccount.Create(customerId, AccountType.Savings, new Money(500, Currency.SAR));

        // Assert
        Assert.NotEqual(Guid.Empty, account.Id);
        Assert.Equal(customerId, account.CustomerId);
        Assert.StartsWith("SA", account.Iban.Value);
        Assert.Equal(24, account.Iban.Value.Length);
        Assert.Equal(500, account.Balance.Amount);
        Assert.Single(account.Transactions);
        Assert.NotEmpty(account.DomainEvents);
    }

    [Fact]
    public void Deposit_ValidAmount_ShouldIncreaseBalance()
    {
        // Arrange
        var account = BankAccount.Create(Guid.NewGuid(), AccountType.Current, new Money(100, Currency.SAR));

        // Act
        account.Deposit(new Money(250, Currency.SAR), "ATM Cash Deposit");

        // Assert
        Assert.Equal(350, account.Balance.Amount);
        Assert.Equal(2, account.Transactions.Count);
    }

    [Fact]
    public void Withdraw_SufficientBalance_ShouldDecreaseBalance()
    {
        // Arrange
        var account = BankAccount.Create(Guid.NewGuid(), AccountType.Current, new Money(1000, Currency.SAR));

        // Act
        account.Withdraw(new Money(400, Currency.SAR), "POS Purchase");

        // Assert
        Assert.Equal(600, account.Balance.Amount);
    }

    [Fact]
    public void Withdraw_InsufficientBalance_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var account = BankAccount.Create(Guid.NewGuid(), AccountType.Current, new Money(100, Currency.SAR));

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => account.Withdraw(new Money(500, Currency.SAR)));
    }
}
