using FinHub.Domain.Entities;
using FinHub.Domain.ValueObjects;

namespace FinHub.Application.Common.Interfaces;

public interface IBankAccountRepository
{
    Task<BankAccount?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<BankAccount?> GetByIbanAsync(IBAN iban, CancellationToken cancellationToken = default);
    Task<List<BankAccount>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task AddAsync(BankAccount account, CancellationToken cancellationToken = default);
    Task UpdateAsync(BankAccount account, CancellationToken cancellationToken = default);
}
