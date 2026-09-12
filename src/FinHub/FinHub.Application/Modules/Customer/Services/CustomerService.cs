using FinHub.Application.Common.Interfaces;
using FinHub.Application.Modules.Customer.DTOs;
using FinHub.Domain.ValueObjects;

namespace FinHub.Application.Modules.Customer.Services;

public sealed class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repository;

    public CustomerService(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<CustomerProfileDto> GetProfileAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var customer = await _repository.GetByIdAsync(customerId, cancellationToken)
            ?? throw new KeyNotFoundException($"Customer with ID '{customerId}' was not found.");

        return MapToDto(customer);
    }

    public async Task<CustomerProfileDto> UpdateAddressAsync(Guid customerId, UpdateCustomerAddressCommand command, CancellationToken cancellationToken = default)
    {
        var customer = await _repository.GetByIdAsync(customerId, cancellationToken)
            ?? throw new KeyNotFoundException($"Customer with ID '{customerId}' was not found.");

        var address = new Address(
            command.BuildingNumber,
            command.Street,
            command.District,
            command.City,
            command.PostalCode,
            command.AdditionalNumber,
            command.Country);

        customer.UpdateAddress(address);
        await _repository.UpdateAsync(customer, cancellationToken);

        return MapToDto(customer);
    }

    public async Task<CustomerProfileDto> SetNationalIdAsync(Guid customerId, string rawNationalId, CancellationToken cancellationToken = default)
    {
        var customer = await _repository.GetByIdAsync(customerId, cancellationToken)
            ?? throw new KeyNotFoundException($"Customer with ID '{customerId}' was not found.");

        var nationalId = new NationalId(rawNationalId);
        customer.SetNationalId(nationalId);
        await _repository.UpdateAsync(customer, cancellationToken);

        return MapToDto(customer);
    }

    private static CustomerProfileDto MapToDto(Domain.Entities.Customer customer)
    {
        AddressDto? addressDto = customer.Address != null
            ? new AddressDto(
                customer.Address.BuildingNumber,
                customer.Address.Street,
                customer.Address.District,
                customer.Address.City,
                customer.Address.PostalCode,
                customer.Address.AdditionalNumber,
                customer.Address.Country)
            : null;

        return new CustomerProfileDto(
            customer.Id,
            customer.Email.Value,
            customer.FullName,
            customer.Role.ToString(),
            customer.NationalId?.Value,
            customer.NationalId?.IsCitizen ?? false,
            addressDto,
            customer.IsEmailVerified,
            customer.IsMfaEnabled,
            customer.CreatedAt);
    }
}
