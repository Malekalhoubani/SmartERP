using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartERP.Organization.DataAccess.Configurations.Common;
using SmartERP.Organization.Domain.Entities;

namespace SmartERP.Organization.DataAccess.Configurations;

public sealed class CompanyConfiguration
    : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("Companies","organization");

        builder.ConfigureAuditableEntity();

        builder.Property(company => company.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(company => company.LegalName)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(company => company.Code)
            .HasMaxLength(50)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(company => company.RegistrationNumber)
            .HasMaxLength(100);

        builder.Property(company => company.TaxNumber)
            .HasMaxLength(100);

        builder.Property(company => company.Email)
            .HasMaxLength(256);

        builder.Property(company => company.Phone)
            .HasMaxLength(30);

        builder.Property(company => company.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.HasIndex(company => company.Code)
            .IsUnique();

        builder.HasIndex(company => company.RegistrationNumber)
            .IsUnique()
            .HasFilter("[RegistrationNumber] IS NOT NULL");

        builder.HasIndex(company => company.TaxNumber)
            .IsUnique()
            .HasFilter("[TaxNumber] IS NOT NULL");

        builder.HasIndex(company => company.Email);

        builder.HasIndex(company => company.IsActive);
    }
}