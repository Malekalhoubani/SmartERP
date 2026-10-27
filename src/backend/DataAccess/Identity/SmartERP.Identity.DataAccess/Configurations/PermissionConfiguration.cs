using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartERP.Identity.Domain.Entities;

namespace SmartERP.Identity.DataAccess.Configurations;

public sealed class PermissionConfiguration: IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions", "identity");

        builder.HasKey(permission => permission.Id);

        builder.Property(permission => permission.Id)
            .ValueGeneratedOnAdd();

        builder.Property(permission => permission.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(permission => permission.DisplayName)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(permission => permission.Module)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(permission => permission.Description)
            .HasMaxLength(300);

        builder.Property(permission => permission.IsActive)
            .HasDefaultValue(true);

        builder.Property(permission => permission.CreatedAtUtc)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        builder.HasIndex(permission => permission.Name)
            .IsUnique();

        builder.HasIndex(permission => permission.Module);
    }
}