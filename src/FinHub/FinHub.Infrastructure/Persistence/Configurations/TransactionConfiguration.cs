using FinHub.Domain.Entities;
using FinHub.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinHub.Infrastructure.Persistence.Configurations;

public sealed class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.AccountId)
            .IsRequired();

        builder.Property(t => t.Type)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(t => t.Description)
            .HasMaxLength(256);

        builder.Property(t => t.ReferenceNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.OwnsOne(t => t.Amount, amount =>
        {
            amount.Property(m => m.Amount).HasColumnName("Amount").HasPrecision(18, 2);
            amount.Property(m => m.Currency).HasColumnName("Currency").HasConversion<string>().HasMaxLength(3);
        });
    }
}
