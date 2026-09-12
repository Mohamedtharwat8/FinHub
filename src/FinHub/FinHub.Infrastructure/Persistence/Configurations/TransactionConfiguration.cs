using FinHub.Domain.Entities;
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

        builder.Property(t => t.AmountValue)
            .HasColumnName("Amount")
            .HasPrecision(18, 2);

        builder.Property(t => t.Currency)
            .HasColumnName("Currency")
            .HasConversion<string>()
            .HasMaxLength(3);

        builder.Ignore(t => t.Amount);
    }
}
