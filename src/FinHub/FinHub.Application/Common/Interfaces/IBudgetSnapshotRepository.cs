using FinHub.Domain.Entities;

namespace FinHub.Application.Common.Interfaces;

public interface IBudgetSnapshotRepository
{
    Task<BudgetSnapshot?> GetLatestByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task AddAsync(BudgetSnapshot budgetSnapshot, CancellationToken cancellationToken = default);
    Task UpdateAsync(BudgetSnapshot budgetSnapshot, CancellationToken cancellationToken = default);
}
