using FinHub.Application.Common.Interfaces;
using FinHub.Application.Modules.Banking.DTOs;
using FinHub.Domain.Entities;
using FinHub.Domain.Enums;
using FinHub.Domain.ValueObjects;

namespace FinHub.Application.Modules.Banking.Services;

public sealed class BankingService : IBankingService
{
    private readonly IBankAccountRepository _accountRepository;
    private readonly ICustomerRepository _customerRepository;

    public BankingService(IBankAccountRepository accountRepository, ICustomerRepository customerRepository)
    {
        _accountRepository = accountRepository;
        _customerRepository = customerRepository;
    }

    public async Task<BankAccountDto> CreateAccountAsync(CreateAccountCommand command, CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdAsync(command.CustomerId, cancellationToken)
            ?? throw new KeyNotFoundException($"Customer with ID '{command.CustomerId}' was not found.");

        if (!Enum.TryParse<AccountType>(command.Type, true, out var type))
        {
            throw new ArgumentException($"Invalid account type '{command.Type}'. Valid types: Savings, Current, Investment, Credit.");
        }

        if (!Enum.TryParse<Currency>(command.Currency, true, out var currency))
        {
            throw new ArgumentException($"Invalid currency '{command.Currency}'. Valid currencies: SAR, USD, EUR, GBP, AED.");
        }

        var initialBalance = command.InitialDeposit > 0 ? new Money(command.InitialDeposit, currency) : null;
        var account = BankAccount.Create(customer.Id, type, initialBalance);

        await _accountRepository.AddAsync(account, cancellationToken);

        return MapToDto(account);
    }

    public async Task<BankAccountDto> GetAccountByIdAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetByIdAsync(accountId, cancellationToken)
            ?? throw new KeyNotFoundException($"Bank account with ID '{accountId}' was not found.");

        return MapToDto(account);
    }

    public async Task<List<BankAccountDto>> GetAccountsByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var accounts = await _accountRepository.GetByCustomerIdAsync(customerId, cancellationToken);
        return accounts.Select(MapToDto).ToList();
    }

    public async Task<BankAccountDto> DepositAsync(Guid accountId, DepositCommand command, CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetByIdAsync(accountId, cancellationToken)
            ?? throw new KeyNotFoundException($"Bank account with ID '{accountId}' was not found.");

        if (!Enum.TryParse<Currency>(command.Currency, true, out var currency))
        {
            throw new ArgumentException($"Invalid currency '{command.Currency}'.");
        }

        var depositMoney = new Money(command.Amount, currency);
        account.Deposit(depositMoney, command.Description);

        await _accountRepository.UpdateAsync(account, cancellationToken);

        return MapToDto(account);
    }

    public async Task<BankAccountDto> WithdrawAsync(Guid accountId, WithdrawCommand command, CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetByIdAsync(accountId, cancellationToken)
            ?? throw new KeyNotFoundException($"Bank account with ID '{accountId}' was not found.");

        if (!Enum.TryParse<Currency>(command.Currency, true, out var currency))
        {
            throw new ArgumentException($"Invalid currency '{command.Currency}'.");
        }

        var withdrawMoney = new Money(command.Amount, currency);
        account.Withdraw(withdrawMoney, command.Description);

        await _accountRepository.UpdateAsync(account, cancellationToken);

        return MapToDto(account);
    }

    public async Task<List<TransactionDto>> GetTransactionsAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetByIdAsync(accountId, cancellationToken)
            ?? throw new KeyNotFoundException($"Bank account with ID '{accountId}' was not found.");

        return account.Transactions
            .OrderByDescending(t => t.TransactionDate)
            .Select(t => new TransactionDto
            {
                Id = t.Id,
                AccountId = t.AccountId,
                Type = t.Type.ToString(),
                Amount = t.Amount.Amount,
                Currency = t.Amount.Currency.ToString(),
                Description = t.Description,
                ReferenceNumber = t.ReferenceNumber,
                TransactionDate = t.TransactionDate
            })
            .ToList();
    }

    public async Task<List<TransactionDto>> GetCustomerTransactionsAsync(Guid customerId, int page = 1, int pageSize = 25, CancellationToken cancellationToken = default)
    {
        if (page < 1)
            page = 1;

        if (pageSize < 1)
            pageSize = 25;

        var customer = await _customerRepository.GetByIdAsync(customerId, cancellationToken)
            ?? throw new KeyNotFoundException($"Customer with ID '{customerId}' was not found.");

        var accounts = await _accountRepository.GetByCustomerIdAsync(customer.Id, cancellationToken);
        var transactions = accounts
            .SelectMany(a => a.Transactions)
            .OrderByDescending(t => t.TransactionDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TransactionDto
            {
                Id = t.Id,
                AccountId = t.AccountId,
                Type = t.Type.ToString(),
                Amount = t.Amount.Amount,
                Currency = t.Amount.Currency.ToString(),
                Description = t.Description,
                ReferenceNumber = t.ReferenceNumber,
                TransactionDate = t.TransactionDate
            })
            .ToList();

        return transactions;
    }

    private static BankAccountDto MapToDto(BankAccount account)
    {
        return new BankAccountDto
        {
            Id = account.Id,
            CustomerId = account.CustomerId,
            Iban = account.Iban.Value,
            AccountNumber = account.AccountNumber,
            Type = account.Type.ToString(),
            Status = account.Status.ToString(),
            BalanceAmount = account.Balance.Amount,
            Currency = account.Balance.Currency.ToString(),
            CreatedAt = account.CreatedAt
        };
    }
}
