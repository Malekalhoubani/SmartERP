using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartERP.Organization.DataAccess.Configurations.Common;
using SmartERP.Organization.Domain.Entities;

namespace SmartERP.Organization.DataAccess.Configurations;

public sealed class TaxConfiguration
    : IEntityTypeConfiguration<Tax>
{
    public void Configure(EntityTypeBuilder<Tax> builder)
    {
        builder.ToTable(
            "Taxes",
            "organization");

        builder.ConfigureAuditableEntity();

        builder.Property(tax => tax.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(tax => tax.Code)
            .HasMaxLength(50)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(tax => tax.RatePercentage)
            .HasPrecision(9, 4)
            .IsRequired();

        builder.Property(tax => tax.CalculationMethod)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(tax => tax.EffectiveFrom)
            .IsRequired();

        builder.Property(tax => tax.EffectiveTo)
            .IsRequired(false);

        builder.Property(tax => tax.IsDefault)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(tax => tax.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.HasOne(tax => tax.Company)
            .WithMany(company => company.Taxes)
            .HasForeignKey(tax => tax.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(tax => new
        {
            tax.CompanyId,
            tax.Code
        })
            .IsUnique();

        builder.HasIndex(tax => new
        {
            tax.CompanyId,
            tax.IsDefault
        })
            .IsUnique()
            .HasFilter("[IsDefault] = 1");

        builder.HasIndex(tax => new
        {
            tax.CompanyId,
            tax.EffectiveFrom,
            tax.EffectiveTo
        });

        builder.HasIndex(tax => tax.IsActive);
    }
}