namespace FinHub.Domain.Entities;

public sealed class BudgetSnapshot
{
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public DateTimeOffset Month { get; private set; }
    public decimal MonthlyBudget { get; private set; }
    public decimal MonthlySpent { get; private set; }
    public decimal RemainingBudget { get; private set; }
    public decimal SavingsGoal { get; private set; }
    public string Currency { get; private set; } = "SAR";
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    private BudgetSnapshot()
    {
    }

    public static BudgetSnapshot Create(
        Guid customerId,
        DateTimeOffset month,
        decimal monthlyBudget,
        decimal monthlySpent,
        decimal remainingBudget,
        decimal savingsGoal,
        string currency = "SAR")
    {
        return new BudgetSnapshot
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            Month = month,
            MonthlyBudget = monthlyBudget,
            MonthlySpent = monthlySpent,
            RemainingBudget = remainingBudget,
            SavingsGoal = savingsGoal,
            Currency = currency,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }
}
