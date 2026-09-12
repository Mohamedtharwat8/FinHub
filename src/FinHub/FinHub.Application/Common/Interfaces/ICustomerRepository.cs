using FinHub.Domain.Entities;
using FinHub.Domain.ValueObjects;

namespace FinHub.Application.Common.Interfaces;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Customer?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);
    Task<Customer?> GetByPhoneNumberAsync(PhoneNumber phoneNumber, CancellationToken cancellationToken = default);
    Task<Customer?> GetByExternalLoginAsync(string provider, string providerKey, CancellationToken cancellationToken = default);
    Task<Customer?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default);
    Task AddAsync(Customer customer, CancellationToken cancellationToken = default);
    Task UpdateAsync(Customer customer, CancellationToken cancellationToken = default);
}
