using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartERP.Inventory.Domain.Entities;

namespace SmartERP.Inventory.DataAccess.Configurations;

public sealed class CatalogItemConfiguration : IEntityTypeConfiguration<CatalogItem>
{
    public void Configure(EntityTypeBuilder<CatalogItem> builder)
    {
        builder.ToTable("CatalogItems","inventory");
        builder.HasKey(item => item.Id);
        builder.Property(item => item.Name).HasMaxLength(160).IsRequired();
        builder.Property(item => item.Code).HasMaxLength(64).IsRequired();
        builder.Property(item => item.Description).HasMaxLength(1000);
        builder.Property(item => item.CurrencyCode).HasMaxLength(3);
        builder.Property(item => item.Amount).HasPrecision(19,4);
        builder.Property(item => item.Rate).HasPrecision(9,4);
        builder.Property(item => item.MetadataJson).HasColumnType("nvarchar(max)");
        builder.HasIndex(item => new { item.CompanyId,item.Type,item.Code }).IsUnique();
        builder.HasIndex(item => new { item.CompanyId,item.Type,item.IsActive });
    }
}
