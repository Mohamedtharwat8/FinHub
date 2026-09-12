using FinHub.Application.Common.Interfaces;
using FinHub.Application.Modules.Identity.DTOs;
using CustomerEntity = FinHub.Domain.Entities.Customer;
using FinHub.Domain.Enums;
using FinHub.Domain.ValueObjects;

namespace FinHub.Application.Modules.Identity.Services;

public sealed class IdentityService : IIdentityService
{
    private readonly ICustomerRepository _repository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly ITotpService _totpService;

    public IdentityService(
        ICustomerRepository repository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        ITotpService totpService)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _totpService = totpService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var email = new Email(request.Email);
        if (await _repository.ExistsByEmailAsync(email, cancellationToken))
        {
            throw new InvalidOperationException($"Customer with email '{request.Email}' already exists.");
        }

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
        {
            throw new ArgumentException("Password must be at least 8 characters long.", nameof(request.Password));
        }

        var passwordHash = _passwordHasher.HashPassword(request.Password);
        var customer = CustomerEntity.Create(email, request.FullName, passwordHash, UserRole.Customer);

        var (token, expiration) = _jwtTokenGenerator.GenerateAccessToken(customer);
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();
        customer.SetRefreshToken(refreshToken, DateTimeOffset.UtcNow.AddDays(7));

        await _repository.AddAsync(customer, cancellationToken);

        return new AuthResponse(
            customer.Id,
            customer.Email.Value,
            customer.FullName,
            token,
            refreshToken,
            expiration);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var email = new Email(request.Email);
        var customer = await _repository.GetByEmailAsync(email, cancellationToken);
        if (customer == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        if (customer.IsLockedOut())
        {
            throw new UnauthorizedAccessException($"Account locked out until {customer.LockoutEnd}.");
        }

        if (!_passwordHasher.VerifyPassword(request.Password, customer.PasswordHash))
        {
            customer.RecordFailedLoginAttempt();
            await _repository.UpdateAsync(customer, cancellationToken);
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        if (customer.IsMfaEnabled)
        {
            if (string.IsNullOrWhiteSpace(request.TotpCode) || !_totpService.VerifyCode(customer.MfaSecret ?? "", request.TotpCode))
            {
                return new AuthResponse(customer.Id, customer.Email.Value, customer.FullName, "", "", DateTimeOffset.UtcNow, RequiresMfa: true);
            }
        }

        customer.ResetFailedLoginCount();
        var (token, expiration) = _jwtTokenGenerator.GenerateAccessToken(customer);
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();
        customer.SetRefreshToken(refreshToken, DateTimeOffset.UtcNow.AddDays(7));

        await _repository.UpdateAsync(customer, cancellationToken);

        return new AuthResponse(
            customer.Id,
            customer.Email.Value,
            customer.FullName,
            token,
            refreshToken,
            expiration);
    }

    public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        var customer = await _repository.GetByRefreshTokenAsync(request.RefreshToken, cancellationToken);
        if (customer == null || customer.RefreshTokenExpiryTime <= DateTimeOffset.UtcNow)
        {
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");
        }

        var (token, expiration) = _jwtTokenGenerator.GenerateAccessToken(customer);
        var newRefreshToken = _jwtTokenGenerator.GenerateRefreshToken();
        customer.SetRefreshToken(newRefreshToken, DateTimeOffset.UtcNow.AddDays(7));

        await _repository.UpdateAsync(customer, cancellationToken);

        return new AuthResponse(
            customer.Id,
            customer.Email.Value,
            customer.FullName,
            token,
            newRefreshToken,
            expiration);
    }
}
