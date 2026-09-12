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
    private readonly IOtpService _otpService;
    private readonly IOAuthService _oAuthService;

    public IdentityService(
        ICustomerRepository repository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        ITotpService totpService,
        IOtpService otpService,
        IOAuthService oAuthService)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _totpService = totpService;
        _otpService = otpService;
        _oAuthService = oAuthService;
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

    public async Task<AuthResponse> ExternalLoginAsync(ExternalLoginRequest request, CancellationToken cancellationToken = default)
    {
        var userInfo = await _oAuthService.ValidateTokenAsync(request.Provider, request.Token, cancellationToken);

        var existingCustomer = await _repository.GetByExternalLoginAsync(userInfo.Provider, userInfo.ProviderKey, cancellationToken);
        if (existingCustomer != null)
        {
            var (t, exp) = _jwtTokenGenerator.GenerateAccessToken(existingCustomer);
            var r = _jwtTokenGenerator.GenerateRefreshToken();
            existingCustomer.SetRefreshToken(r, DateTimeOffset.UtcNow.AddDays(7));
            await _repository.UpdateAsync(existingCustomer, cancellationToken);

            return new AuthResponse(existingCustomer.Id, existingCustomer.Email.Value, existingCustomer.FullName, t, r, exp);
        }

        var email = new Email(userInfo.Email);
        var customerByEmail = await _repository.GetByEmailAsync(email, cancellationToken);

        if (customerByEmail != null)
        {
            customerByEmail.AddExternalLogin(userInfo.Provider, userInfo.ProviderKey, userInfo.Email);
            var (t, exp) = _jwtTokenGenerator.GenerateAccessToken(customerByEmail);
            var r = _jwtTokenGenerator.GenerateRefreshToken();
            customerByEmail.SetRefreshToken(r, DateTimeOffset.UtcNow.AddDays(7));
            await _repository.UpdateAsync(customerByEmail, cancellationToken);

            return new AuthResponse(customerByEmail.Id, customerByEmail.Email.Value, customerByEmail.FullName, t, r, exp);
        }

        var dummyPassword = _passwordHasher.HashPassword(Guid.NewGuid().ToString("N"));
        var newCustomer = CustomerEntity.Create(email, userInfo.FullName, dummyPassword, UserRole.Customer);
        if (userInfo.EmailVerified)
        {
            newCustomer.VerifyEmail();
        }
        newCustomer.AddExternalLogin(userInfo.Provider, userInfo.ProviderKey, userInfo.Email);

        var (accessToken, expiration) = _jwtTokenGenerator.GenerateAccessToken(newCustomer);
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();
        newCustomer.SetRefreshToken(refreshToken, DateTimeOffset.UtcNow.AddDays(7));

        await _repository.AddAsync(newCustomer, cancellationToken);

        return new AuthResponse(newCustomer.Id, newCustomer.Email.Value, newCustomer.FullName, accessToken, refreshToken, expiration);
    }

    public async Task<string> SendOtpAsync(SendOtpRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Target))
            throw new ArgumentException("Target email or phone number is required.", nameof(request.Target));

        return await _otpService.GenerateOtpAsync(request.Target, request.Purpose, cancellationToken);
    }

    public async Task<bool> VerifyOtpAsync(VerifyOtpRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Target) || string.IsNullOrWhiteSpace(request.Code))
            return false;

        return await _otpService.VerifyOtpAsync(request.Target, request.Code, request.Purpose, cancellationToken);
    }

    public async Task<AuthResponse> RegisterWithPhoneAsync(PhoneRegisterRequest request, CancellationToken cancellationToken = default)
    {
        var phone = new PhoneNumber(request.PhoneNumber);
        var isValid = await _otpService.VerifyOtpAsync(phone.Value, request.OtpCode, "PhoneRegistration", cancellationToken);
        if (!isValid)
        {
            throw new UnauthorizedAccessException("Invalid or expired SMS OTP code.");
        }

        var existingCustomer = await _repository.GetByPhoneNumberAsync(phone, cancellationToken);
        if (existingCustomer != null)
        {
            throw new InvalidOperationException($"Account with phone number '{phone.Value}' already exists.");
        }

        var syntheticEmail = new Email($"user.{phone.Value.Replace("+", "")}@finhub.sa");
        var dummyPassword = _passwordHasher.HashPassword(Guid.NewGuid().ToString("N"));
        var customer = CustomerEntity.Create(syntheticEmail, request.FullName, dummyPassword, UserRole.Customer);
        customer.SetPhoneNumber(phone);
        customer.VerifyPhoneNumber();

        var (token, expiration) = _jwtTokenGenerator.GenerateAccessToken(customer);
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();
        customer.SetRefreshToken(refreshToken, DateTimeOffset.UtcNow.AddDays(7));

        await _repository.AddAsync(customer, cancellationToken);

        return new AuthResponse(customer.Id, customer.Email.Value, customer.FullName, token, refreshToken, expiration);
    }
}
