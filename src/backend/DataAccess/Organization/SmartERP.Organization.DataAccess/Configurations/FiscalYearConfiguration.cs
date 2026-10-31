using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartERP.Organization.DataAccess.Configurations.Common;
using SmartERP.Organization.Domain.Entities;

namespace SmartERP.Organization.DataAccess.Configurations;

public sealed class FiscalYearConfiguration
    : IEntityTypeConfiguration<FiscalYear>
{
    public void Configure(
        EntityTypeBuilder<FiscalYear> builder)
    {
        builder.ToTable(
            "FiscalYears",
            "organization");

        builder.ConfigureAuditableEntity();

        builder.Property(fiscalYear => fiscalYear.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(fiscalYear => fiscalYear.StartDate)
            .IsRequired();

        builder.Property(fiscalYear => fiscalYear.EndDate)
            .IsRequired();

        builder.Property(fiscalYear => fiscalYear.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(fiscalYear => fiscalYear.IsCurrent)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(fiscalYear => fiscalYear.ClosedAtUtc)
            .IsRequired(false);

        builder.Property(fiscalYear => fiscalYear.ClosedByUserId)
            .IsRequired(false);

        builder.HasOne(fiscalYear => fiscalYear.Company)
            .WithMany(company => company.FiscalYears)
            .HasForeignKey(fiscalYear =>
                fiscalYear.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(fiscalYear => new
        {
            fiscalYear.CompanyId,
            fiscalYear.Name
        })
            .IsUnique();

        builder.HasIndex(fiscalYear => new
        {
            fiscalYear.CompanyId,
            fiscalYear.IsCurrent
        })
            .IsUnique()
            .HasFilter("[IsCurrent] = 1");

        builder.HasIndex(fiscalYear => new
        {
            fiscalYear.CompanyId,
            fiscalYear.StartDate,
            fiscalYear.EndDate
        });

        builder.HasIndex(fiscalYear => fiscalYear.Status);
    }
}