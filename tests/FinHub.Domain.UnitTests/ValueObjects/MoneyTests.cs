using FinHub.Domain.ValueObjects;

namespace FinHub.Domain.UnitTests.ValueObjects;

public class MoneyTests
{
    [Fact]
    public void Add_SameCurrency_ShouldReturnSum()
    {
        // Arrange
        var m1 = new Money(100.50m, Currency.SAR);
        var m2 = new Money(50.25m, Currency.SAR);

        // Act
        var result = m1 + m2;

        // Assert
        Assert.Equal(150.75m, result.Amount);
        Assert.Equal(Currency.SAR, result.Currency);
    }

    [Fact]
    public void Add_DifferentCurrencies_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var m1 = new Money(100m, Currency.SAR);
        var m2 = new Money(100m, Currency.USD);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => m1 + m2);
    }

    [Theory]
    [InlineData("SA0380000000608010167519")]
    [InlineData("SA5620000001234567890123")]
    public void ValidIBAN_ShouldInstantiate(string rawIban)
    {
        // Act
        var iban = new IBAN(rawIban);

        // Assert
        Assert.NotNull(iban.Value);
        Assert.Equal(rawIban, iban.Value);
    }
}
