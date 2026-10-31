using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartERP.Organization.DataAccess.Configurations.Common;
using SmartERP.Organization.Domain.Entities;

namespace SmartERP.Organization.DataAccess.Configurations;

public sealed class CompanyCurrencyConfiguration: IEntityTypeConfiguration<CompanyCurrency>
{
    public void Configure(
        EntityTypeBuilder<CompanyCurrency> builder)
    {
        builder.ToTable(
            "CompanyCurrencies",
            "organization");

        builder.ConfigureAuditableEntity();

        builder.Property(companyCurrency =>
                companyCurrency.ExchangeRateToBase)
            .HasPrecision(18, 8)
            .HasDefaultValue(1m)
            .IsRequired();

        builder.Property(companyCurrency =>
                companyCurrency.IsBaseCurrency)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(companyCurrency =>
                companyCurrency.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.HasOne(companyCurrency =>
                companyCurrency.Company)
            .WithMany(company => company.Currencies)
            .HasForeignKey(companyCurrency =>
                companyCurrency.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(companyCurrency =>
                companyCurrency.Currency)
            .WithMany(currency => currency.Companies)
            .HasForeignKey(companyCurrency =>
                companyCurrency.CurrencyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(companyCurrency => new
        {
            companyCurrency.CompanyId,
            companyCurrency.CurrencyId
        })
            .IsUnique();

        builder.HasIndex(companyCurrency => new
        {
            companyCurrency.CompanyId,
            companyCurrency.IsBaseCurrency
        })
            .IsUnique()
            .HasFilter("[IsBaseCurrency] = 1");

        builder.HasIndex(companyCurrency =>
            companyCurrency.IsActive);
    }
}