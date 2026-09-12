using FinHub.Application.Common.Interfaces;
using FinHub.Domain.Entities;
using FinHub.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace FinHub.Infrastructure.Persistence;

public sealed class EfBankAccountRepository : IBankAccountRepository
{
    private readonly FinHubDbContext _context;

    public EfBankAccountRepository(FinHubDbContext context)
    {
        _context = context;
    }

    public async Task<BankAccount?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.BankAccounts
            .Include(b => b.Transactions)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task<BankAccount?> GetByIbanAsync(IBAN iban, CancellationToken cancellationToken = default)
    {
        return await _context.BankAccounts
            .Include(b => b.Transactions)
            .FirstOrDefaultAsync(b => b.Iban == iban, cancellationToken);
    }

    public async Task<List<BankAccount>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _context.BankAccounts
            .Include(b => b.Transactions)
            .Where(b => b.CustomerId == customerId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(BankAccount account, CancellationToken cancellationToken = default)
    {
        await _context.BankAccounts.AddAsync(account, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(BankAccount account, CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
