using FinHub.Domain.Entities;
using FinHub.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinHub.Infrastructure.Persistence.Configurations;

public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.FullName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(c => c.PasswordHash)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(c => c.Role)
            .IsRequired();

        // Email Value Object Mapping
        builder.Property(c => c.Email)
            .HasConversion(email => email.Value, value => new Email(value))
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(c => c.Email)
            .IsUnique();

        // NationalId Value Object Mapping (Optional)
        builder.Property(c => c.NationalId)
            .HasConversion(
                id => id != null ? id.Value : null,
                value => value != null ? new NationalId(value) : null)
            .HasMaxLength(10);

        // Complex Address Owned Type Mapping
        builder.OwnsOne(c => c.Address, address =>
        {
            address.Property(a => a.BuildingNumber).HasColumnName("Address_BuildingNumber").HasMaxLength(10);
            address.Property(a => a.Street).HasColumnName("Address_Street").HasMaxLength(150);
            address.Property(a => a.District).HasColumnName("Address_District").HasMaxLength(100);
            address.Property(a => a.City).HasColumnName("Address_City").HasMaxLength(100);
            address.Property(a => a.PostalCode).HasColumnName("Address_PostalCode").HasMaxLength(10);
            address.Property(a => a.AdditionalNumber).HasColumnName("Address_AdditionalNumber").HasMaxLength(10);
            address.Property(a => a.Country).HasColumnName("Address_Country").HasMaxLength(100);
        });

        builder.Ignore(c => c.DomainEvents);
    }
}
