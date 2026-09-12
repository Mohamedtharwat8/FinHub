namespace FinHub.Application.Modules.Budgeting.DTOs;

public sealed class BudgetOverviewDto
{
    public Guid CustomerId { get; init; }
    public DateTimeOffset Month { get; init; } = DateTimeOffset.UtcNow;
    public decimal MonthlyBudget { get; init; }
    public decimal MonthlySpent { get; init; }
    public decimal RemainingBudget { get; init; }
    public decimal SavingsGoal { get; init; }
    public string Currency { get; init; } = "SAR";
    public List<BudgetAlertDto> Alerts { get; init; } = new();
}

public sealed class BudgetAlertDto
{
    public string Type { get; init; } = "SpendingLimit";
    public string Title { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public decimal Threshold { get; init; }
    public bool IsActive { get; init; }
}
