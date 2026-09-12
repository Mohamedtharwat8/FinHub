using FinHub.Domain.Entities;
using FinHub.Domain.Enums;
using FinHub.Domain.Events;
using FinHub.Domain.ValueObjects;

namespace FinHub.Domain.UnitTests.Entities;

public class CustomerTests
{
    [Fact]
    public void Create_ValidParameters_ShouldInstantiateAndPublishDomainEvent()
    {
        // Arrange
        var email = new Email("ahmed.salem@example.sa");
        var fullName = "Ahmed Salem";
        var passwordHash = "Argon2idHashedPassword123!";

        // Act
        var customer = Customer.Create(email, fullName, passwordHash, UserRole.Customer);

        // Assert
        Assert.NotEqual(Guid.Empty, customer.Id);
        Assert.Equal(email, customer.Email);
        Assert.Equal("Ahmed Salem", customer.FullName);
        Assert.False(customer.IsEmailVerified);
        Assert.False(customer.IsMfaEnabled);
        Assert.Single(customer.DomainEvents);
        Assert.IsType<CustomerRegisteredEvent>(customer.DomainEvents.First());
    }

    [Fact]
    public void RecordFailedLoginAttempt_ReachingThreshold_ShouldLockoutCustomer()
    {
        // Arrange
        var customer = Customer.Create(
            new Email("test@domain.com"),
            "Test User",
            "hashed_pwd");

        // Act
        for (int i = 0; i < 5; i++)
        {
            customer.RecordFailedLoginAttempt(maxFailedAttempts: 5);
        }

        // Assert
        Assert.True(customer.IsLockedOut());
        Assert.Equal(5, customer.AccessFailedCount);
    }

    [Fact]
    public void ResetFailedLoginCount_ShouldClearLockoutAndCounter()
    {
        // Arrange
        var customer = Customer.Create(
            new Email("test@domain.com"),
            "Test User",
            "hashed_pwd");
        customer.RecordFailedLoginAttempt(maxFailedAttempts: 1);

        // Act
        customer.ResetFailedLoginCount();

        // Assert
        Assert.False(customer.IsLockedOut());
        Assert.Equal(0, customer.AccessFailedCount);
    }
}
