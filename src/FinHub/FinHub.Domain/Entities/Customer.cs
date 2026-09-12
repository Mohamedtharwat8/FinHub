using FinHub.Domain.Enums;
using FinHub.Domain.Events;
using FinHub.Domain.ValueObjects;

namespace FinHub.Domain.Entities;

public sealed class Customer
{
    public Guid Id { get; private set; }
    public Email Email { get; private set; } = null!;
    public string FullName { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public UserRole Role { get; private set; }
    public NationalId? NationalId { get; private set; }
    public Address? Address { get; private set; }
    public bool IsEmailVerified { get; private set; }
    public bool IsMfaEnabled { get; private set; }
    public string? MfaSecret { get; set; }
    public string? RefreshToken { get; private set; }
    public DateTimeOffset? RefreshTokenExpiryTime { get; private set; }
    public int AccessFailedCount { get; private set; }
    public DateTimeOffset? LockoutEnd { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    private readonly List<object> _domainEvents = [];
    public IReadOnlyCollection<object> DomainEvents => _domainEvents.AsReadOnly();

    private Customer() { } // EF Core constructor

    public static Customer Create(
        Email email,
        string fullName,
        string passwordHash,
        UserRole role = UserRole.Customer)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name is required.", nameof(fullName));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash is required.", nameof(passwordHash));

        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Email = email,
            FullName = fullName.Trim(),
            PasswordHash = passwordHash,
            Role = role,
            IsEmailVerified = false,
            IsMfaEnabled = false,
            AccessFailedCount = 0,
            CreatedAt = DateTimeOffset.UtcNow
        };

        customer._domainEvents.Add(new CustomerRegisteredEvent(
            customer.Id,
            customer.Email,
            customer.FullName,
            customer.CreatedAt));

        return customer;
    }

    public void VerifyEmail()
    {
        IsEmailVerified = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void SetNationalId(NationalId nationalId)
    {
        NationalId = nationalId;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void UpdateAddress(Address address)
    {
        Address = address;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void EnableMfa(string secret)
    {
        if (string.IsNullOrWhiteSpace(secret))
            throw new ArgumentException("MFA secret cannot be empty.", nameof(secret));

        MfaSecret = secret;
        IsMfaEnabled = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void SetRefreshToken(string refreshToken, DateTimeOffset expiryTime)
    {
        RefreshToken = refreshToken;
        RefreshTokenExpiryTime = expiryTime;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void RevokeRefreshToken()
    {
        RefreshToken = null;
        RefreshTokenExpiryTime = null;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void RecordFailedLoginAttempt(int maxFailedAttempts = 5, TimeSpan? lockoutDuration = null)
    {
        AccessFailedCount++;
        if (AccessFailedCount >= maxFailedAttempts)
        {
            LockoutEnd = DateTimeOffset.UtcNow.Add(lockoutDuration ?? TimeSpan.FromMinutes(15));
        }
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void ResetFailedLoginCount()
    {
        AccessFailedCount = 0;
        LockoutEnd = null;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public bool IsLockedOut() => LockoutEnd.HasValue && LockoutEnd.Value > DateTimeOffset.UtcNow;

    public void ClearDomainEvents() => _domainEvents.Clear();
}
