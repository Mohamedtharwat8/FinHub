using FinHub.Domain.Entities;
using FinHub.Domain.Enums;
using FinHub.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinHub.Infrastructure.Persistence.Configurations;

public sealed class BankAccountConfiguration : IEntityTypeConfiguration<BankAccount>
{
    public void Configure(EntityTypeBuilder<BankAccount> builder)
    {
        builder.ToTable("BankAccounts");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.CustomerId)
            .IsRequired();

        builder.Property(b => b.AccountNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(b => b.Iban)
            .HasConversion(
                iban => iban.Value,
                value => new IBAN(value))
            .IsRequired()
            .HasMaxLength(34);

        builder.Property(b => b.Type)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(b => b.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.OwnsOne(b => b.Balance, balance =>
        {
            balance.Property(m => m.Amount).HasColumnName("Balance_Amount").HasPrecision(18, 2);
            balance.Property(m => m.Currency).HasColumnName("Balance_Currency").HasConversion<string>().HasMaxLength(3);
        });

        builder.HasMany(b => b.Transactions)
            .WithOne()
            .HasForeignKey(t => t.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(b => b.DomainEvents);
    }
}
