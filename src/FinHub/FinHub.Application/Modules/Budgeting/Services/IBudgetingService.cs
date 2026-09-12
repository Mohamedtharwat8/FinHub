using FinHub.Application.Modules.Budgeting.DTOs;

namespace FinHub.Application.Modules.Budgeting.Services;

public interface IBudgetingService
{
    Task<BudgetOverviewDto> GetCustomerBudgetOverviewAsync(Guid customerId, CancellationToken cancellationToken = default);
}
