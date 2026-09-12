using FinHub.Application.Modules.Customer.DTOs;

namespace FinHub.Application.Modules.Customer.Services;

public interface ICustomerService
{
    Task<CustomerProfileDto> GetProfileAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<CustomerProfileDto> UpdateAddressAsync(Guid customerId, UpdateCustomerAddressCommand command, CancellationToken cancellationToken = default);
    Task<CustomerProfileDto> SetNationalIdAsync(Guid customerId, string nationalId, CancellationToken cancellationToken = default);
}
