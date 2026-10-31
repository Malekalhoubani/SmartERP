using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartERP.Organization.Domain.Common;

namespace SmartERP.Organization.DataAccess.Configurations.Common;

public static class AuditableEntityConfigurationExtensions
{
    public static void ConfigureAuditableEntity<TEntity>(
        this EntityTypeBuilder<TEntity> builder)
        where TEntity : AuditableEntity
    {
        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.Id)
            .ValueGeneratedOnAdd();

        builder.Property(entity => entity.CreatedAtUtc)
            .IsRequired();

        builder.Property(entity => entity.CreatedByUserId)
            .IsRequired();

        builder.Property(entity => entity.UpdatedAtUtc)
            .IsRequired(false);

        builder.Property(entity => entity.UpdatedByUserId)
            .IsRequired(false);

        builder.HasIndex(entity => entity.CreatedAtUtc);
    }
}