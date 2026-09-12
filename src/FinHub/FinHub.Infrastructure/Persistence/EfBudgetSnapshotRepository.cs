using FinHub.Application.Common.Interfaces;
using FinHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinHub.Infrastructure.Persistence;

public sealed class EfBudgetSnapshotRepository : IBudgetSnapshotRepository
{
    private readonly FinHubDbContext _context;

    public EfBudgetSnapshotRepository(FinHubDbContext context)
    {
        _context = context;
    }

    public async Task<BudgetSnapshot?> GetLatestByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _context.BudgetSnapshots
            .Where(b => b.CustomerId == customerId)
            .OrderByDescending(b => b.Month)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task AddAsync(BudgetSnapshot budgetSnapshot, CancellationToken cancellationToken = default)
    {
        await _context.BudgetSnapshots.AddAsync(budgetSnapshot, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(BudgetSnapshot budgetSnapshot, CancellationToken cancellationToken = default)
    {
        _context.BudgetSnapshots.Update(budgetSnapshot);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
