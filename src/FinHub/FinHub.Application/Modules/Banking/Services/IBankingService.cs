using FinHub.Application.Modules.Banking.DTOs;

namespace FinHub.Application.Modules.Banking.Services;

public interface IBankingService
{
    Task<BankAccountDto> CreateAccountAsync(CreateAccountCommand command, CancellationToken cancellationToken = default);
    Task<BankAccountDto> GetAccountByIdAsync(Guid accountId, CancellationToken cancellationToken = default);
    Task<List<BankAccountDto>> GetAccountsByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<BankAccountDto> DepositAsync(Guid accountId, DepositCommand command, CancellationToken cancellationToken = default);
    Task<BankAccountDto> WithdrawAsync(Guid accountId, WithdrawCommand command, CancellationToken cancellationToken = default);
    Task<List<TransactionDto>> GetTransactionsAsync(Guid accountId, CancellationToken cancellationToken = default);
}
