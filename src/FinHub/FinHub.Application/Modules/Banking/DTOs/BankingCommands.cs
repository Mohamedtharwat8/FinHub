namespace FinHub.Application.Modules.Banking.DTOs;

public sealed record CreateAccountCommand(
    Guid CustomerId,
    string Type,
    decimal InitialDeposit = 0,
    string Currency = "SAR"
);

public sealed record DepositCommand(
    decimal Amount,
    string Currency = "SAR",
    string Description = "Account Deposit"
);

public sealed record WithdrawCommand(
    decimal Amount,
    string Currency = "SAR",
    string Description = "Account Withdrawal"
);
