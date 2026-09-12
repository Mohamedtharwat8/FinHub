using FinHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinHub.Infrastructure.Persistence.Configurations;

public sealed class BudgetSnapshotConfiguration : IEntityTypeConfiguration<BudgetSnapshot>
{
    public void Configure(EntityTypeBuilder<BudgetSnapshot> builder)
    {
        builder.ToTable("BudgetSnapshots");
        builder.HasKey(b => b.Id);

        builder.Property(b => b.CustomerId)
            .IsRequired();

        builder.Property(b => b.Month)
            .IsRequired();

        builder.Property(b => b.MonthlyBudget)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(b => b.MonthlySpent)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(b => b.RemainingBudget)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(b => b.SavingsGoal)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(b => b.Currency)
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(b => b.CreatedAt)
            .IsRequired();

        builder.Property(b => b.UpdatedAt)
            .IsRequired(false);
    }
}
