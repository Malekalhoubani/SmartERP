using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartERP.Organization.DataAccess.Configurations.Common;
using SmartERP.Organization.Domain.Entities;

namespace SmartERP.Organization.DataAccess.Configurations;

public sealed class CompanyMembershipConfiguration
    : IEntityTypeConfiguration<CompanyMembership>
{
    public void Configure(
        EntityTypeBuilder<CompanyMembership> builder)
    {
        builder.ToTable(
            "CompanyMemberships",
            "organization");

        builder.ConfigureAuditableEntity();

        builder.Property(membership => membership.UserId)
            .IsRequired();

        builder.Property(membership => membership.Role)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(membership => membership.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.HasOne(membership => membership.Company)
            .WithMany(company => company.Memberships)
            .HasForeignKey(membership => membership.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(membership => new
        {
            membership.CompanyId,
            membership.UserId
        })
            .IsUnique();

        builder.HasIndex(membership => membership.UserId);

        builder.HasIndex(membership => membership.IsActive);
    }
}