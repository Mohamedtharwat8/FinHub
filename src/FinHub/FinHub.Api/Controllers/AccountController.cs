using FinHub.Application.Modules.Banking.DTOs;
using FinHub.Application.Modules.Banking.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinHub.Api.Controllers;

[ApiController]
[Route("api/v1/accounts")]
public sealed class AccountController : ControllerBase
{
    private readonly IBankingService _bankingService;

    public AccountController(IBankingService bankingService)
    {
        _bankingService = bankingService;
    }

    [HttpPost]
    public async Task<ActionResult<BankAccountDto>> CreateAccount([FromBody] CreateAccountCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var account = await _bankingService.CreateAccountAsync(command, cancellationToken);
            return CreatedAtAction(nameof(GetAccountById), new { accountId = account.Id }, account);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{accountId:guid}")]
    public async Task<ActionResult<BankAccountDto>> GetAccountById(Guid accountId, CancellationToken cancellationToken)
    {
        try
        {
            var account = await _bankingService.GetAccountByIdAsync(accountId, cancellationToken);
            return Ok(account);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpGet("customer/{customerId:guid}")]
    public async Task<ActionResult<List<BankAccountDto>>> GetAccountsByCustomerId(Guid customerId, CancellationToken cancellationToken)
    {
        var accounts = await _bankingService.GetAccountsByCustomerIdAsync(customerId, cancellationToken);
        return Ok(accounts);
    }

    [HttpPost("{accountId:guid}/deposit")]
    public async Task<ActionResult<BankAccountDto>> Deposit(Guid accountId, [FromBody] DepositCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var account = await _bankingService.DepositAsync(accountId, command, cancellationToken);
            return Ok(account);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{accountId:guid}/withdraw")]
    public async Task<ActionResult<BankAccountDto>> Withdraw(Guid accountId, [FromBody] WithdrawCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var account = await _bankingService.WithdrawAsync(accountId, command, cancellationToken);
            return Ok(account);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{accountId:guid}/transactions")]
    public async Task<ActionResult<List<TransactionDto>>> GetTransactions(Guid accountId, CancellationToken cancellationToken)
    {
        try
        {
            var transactions = await _bankingService.GetTransactionsAsync(accountId, cancellationToken);
            return Ok(transactions);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpGet("customer/{customerId:guid}/transactions")]
    public async Task<ActionResult<List<TransactionDto>>> GetCustomerTransactions(Guid customerId, [FromQuery] int page = 1, [FromQuery] int pageSize = 25, CancellationToken cancellationToken = default)
    {
        try
        {
            var transactions = await _bankingService.GetCustomerTransactionsAsync(customerId, page, pageSize, cancellationToken);
            return Ok(transactions);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
}
