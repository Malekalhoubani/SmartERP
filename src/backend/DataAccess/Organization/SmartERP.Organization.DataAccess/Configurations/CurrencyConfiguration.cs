using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartERP.Organization.DataAccess.Configurations.Common;
using SmartERP.Organization.Domain.Entities;

namespace SmartERP.Organization.DataAccess.Configurations;

public sealed class CurrencyConfiguration
    : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> builder)
    {
        builder.ToTable(
            "Currencies",
            "organization");

        builder.ConfigureAuditableEntity();

        builder.Property(currency => currency.Code)
            .HasMaxLength(3)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(currency => currency.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(currency => currency.Symbol)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(currency => currency.DecimalPlaces)
            .HasDefaultValue(2)
            .IsRequired();

        builder.Property(currency => currency.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.HasIndex(currency => currency.Code)
            .IsUnique();

        builder.HasIndex(currency => currency.IsActive);
    }
}