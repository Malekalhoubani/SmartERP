using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartERP.Identity.Domain.Entities;

namespace SmartERP.Identity.DataAccess.Configurations;

public sealed class ApplicationRoleConfiguration: IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(
        EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.ToTable("Roles", "identity");

        builder.Property(role => role.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(role => role.NormalizedName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(role => role.Description)
            .HasMaxLength(250)
            .IsRequired(false);

        builder.Property(role => role.IsActive)
            .HasDefaultValue(true);

        builder.Property(role => role.IsSystem)
    .HasDefaultValue(false);


        builder.Property(role => role.CreatedAtUtc)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        builder.HasIndex(role => role.NormalizedName)
            .IsUnique()
            .HasDatabaseName("IX_Roles_NormalizedName");
    }
}