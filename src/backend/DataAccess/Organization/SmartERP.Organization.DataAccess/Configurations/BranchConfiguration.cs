using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartERP.Organization.DataAccess.Configurations.Common;
using SmartERP.Organization.Domain.Entities;

namespace SmartERP.Organization.DataAccess.Configurations;

public sealed class BranchConfiguration
    : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.ToTable(
            "Branches",
            "organization");

        builder.ConfigureAuditableEntity();

        builder.Property(branch => branch.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(branch => branch.Code)
            .HasMaxLength(50)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(branch => branch.Email)
            .HasMaxLength(256);

        builder.Property(branch => branch.Phone)
            .HasMaxLength(30);

        builder.Property(branch => branch.AddressLine1)
            .HasMaxLength(250);

        builder.Property(branch => branch.AddressLine2)
            .HasMaxLength(250);

        builder.Property(branch => branch.City)
            .HasMaxLength(100);

        builder.Property(branch => branch.StateOrProvince)
            .HasMaxLength(100);

        builder.Property(branch => branch.PostalCode)
            .HasMaxLength(20);

        builder.Property(branch => branch.CountryCode)
            .HasMaxLength(2)
            .IsUnicode(false);

        builder.Property(branch => branch.IsMainBranch)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(branch => branch.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.HasOne(branch => branch.Company)
            .WithMany(company => company.Branches)
            .HasForeignKey(branch => branch.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(branch => new
        {
            branch.CompanyId,
            branch.Code
        })
            .IsUnique();

        builder.HasIndex(branch => branch.CompanyId);

        builder.HasIndex(branch => new
        {
            branch.CompanyId,
            branch.IsMainBranch
        })
            .IsUnique()
            .HasFilter("[IsMainBranch] = 1");

        builder.HasIndex(branch => branch.IsActive);
    }
}