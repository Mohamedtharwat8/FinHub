using System.Collections.Concurrent;
using FinHub.Application.Common.Interfaces;
using FinHub.Domain.Entities;
using FinHub.Domain.ValueObjects;

namespace FinHub.Infrastructure.Persistence;

public sealed class CustomerRepository : ICustomerRepository
{
    private static readonly ConcurrentDictionary<Guid, Customer> Storage = new();

    public Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Storage.TryGetValue(id, out var customer);
        return Task.FromResult(customer);
    }

    public Task<Customer?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        var customer = Storage.Values.FirstOrDefault(c => c.Email == email);
        return Task.FromResult(customer);
    }

    public Task<Customer?> GetByPhoneNumberAsync(PhoneNumber phoneNumber, CancellationToken cancellationToken = default)
    {
        var customer = Storage.Values.FirstOrDefault(c => c.PhoneNumber == phoneNumber);
        return Task.FromResult(customer);
    }

    public Task<Customer?> GetByExternalLoginAsync(string provider, string providerKey, CancellationToken cancellationToken = default)
    {
        var customer = Storage.Values.FirstOrDefault(c => c.ExternalLogins.Any(x => x.Provider.Equals(provider, StringComparison.OrdinalIgnoreCase) && x.ProviderKey == providerKey));
        return Task.FromResult(customer);
    }

    public Task<Customer?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var customer = Storage.Values.FirstOrDefault(c => c.RefreshToken == refreshToken && c.RefreshTokenExpiryTime > DateTimeOffset.UtcNow);
        return Task.FromResult(customer);
    }

    public Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        var exists = Storage.Values.Any(c => c.Email.Value.Equals(email.Value, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(exists);
    }

    public Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        Storage[customer.Id] = customer;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        Storage[customer.Id] = customer;
        return Task.CompletedTask;
    }
}
