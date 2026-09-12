using FinHub.Application.Common.Interfaces;
using FinHub.Application.Modules.Budgeting.DTOs;
using FinHub.Domain.Entities;

namespace FinHub.Application.Modules.Budgeting.Services;

public sealed class BudgetingService : IBudgetingService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IBankAccountRepository _bankAccountRepository;
    private readonly IBudgetSnapshotRepository _budgetSnapshotRepository;

    public BudgetingService(
        ICustomerRepository customerRepository,
        IBankAccountRepository bankAccountRepository,
        IBudgetSnapshotRepository budgetSnapshotRepository)
    {
        _customerRepository = customerRepository;
        _bankAccountRepository = bankAccountRepository;
        _budgetSnapshotRepository = budgetSnapshotRepository;
    }

    public async Task<BudgetOverviewDto> GetCustomerBudgetOverviewAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdAsync(customerId, cancellationToken)
            ?? throw new KeyNotFoundException($"Customer with ID '{customerId}' was not found.");

        var accounts = await _bankAccountRepository.GetByCustomerIdAsync(customerId, cancellationToken);
        var totalBalance = accounts.Sum(a => a.Balance.Amount);
        var monthlyBudget = Math.Max(totalBalance, 5000m);
        var monthlySpent = Math.Max(totalBalance * 0.32m, 100m);
        var remainingBudget = monthlyBudget - monthlySpent;
        var savingsGoal = Math.Max(monthlyBudget * 0.15m, 250m);
        var month = DateTimeOffset.UtcNow;

        var snapshot = BudgetSnapshot.Create(
            customer.Id,
            month,
            monthlyBudget,
            monthlySpent,
            remainingBudget,
            savingsGoal,
            "SAR");

        await _budgetSnapshotRepository.AddAsync(snapshot, cancellationToken);

        return new BudgetOverviewDto
        {
            CustomerId = customer.Id,
            Month = month,
            MonthlyBudget = monthlyBudget,
            MonthlySpent = monthlySpent,
            RemainingBudget = remainingBudget,
            SavingsGoal = savingsGoal,
            Currency = "SAR",
            Alerts = new List<BudgetAlertDto>
            {
                new()
                {
                    Type = "SpendingLimit",
                    Title = "Monthly spending alert",
                    Message = "You are tracking month-to-date budget usage.",
                    Threshold = monthlyBudget * 0.8m,
                    IsActive = true
                }
            }
        };
    }
}
