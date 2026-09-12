namespace FinHub.Domain.ValueObjects;

public sealed record Money
{
    public decimal Amount { get; }
    public Currency Currency { get; }

    public Money(decimal amount, Currency currency)
    {
        if (amount < 0 && Math.Abs(amount) > 1_000_000_000m)
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount exceeds domain boundaries.");

        Amount = decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
        Currency = currency;
    }

    public static Money Zero(Currency currency) => new(0m, currency);

    public static Money operator +(Money a, Money b)
    {
        if (a.Currency != b.Currency)
            throw new InvalidOperationException($"Cannot add money of different currencies: {a.Currency} vs {b.Currency}");
        return new Money(a.Amount + b.Amount, a.Currency);
    }

    public static Money operator -(Money a, Money b)
    {
        if (a.Currency != b.Currency)
            throw new InvalidOperationException($"Cannot subtract money of different currencies: {a.Currency} vs {b.Currency}");
        return new Money(a.Amount - b.Amount, a.Currency);
    }
}
