using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartERP.Inventory.Domain.Entities;

namespace SmartERP.Inventory.DataAccess.Configurations;

public sealed class OperationalRecordConfiguration : IEntityTypeConfiguration<OperationalRecord>
{
    public void Configure(EntityTypeBuilder<OperationalRecord> builder)
    {
        builder.ToTable("OperationalRecords","inventory");
        builder.HasKey(record => record.Id);
        builder.Property(record => record.Name).HasMaxLength(200).IsRequired();
        builder.Property(record => record.Code).HasMaxLength(64).IsRequired();
        builder.Property(record => record.Description).HasMaxLength(2000);
        builder.Property(record => record.ReferenceNumber).HasMaxLength(100);
        builder.Property(record => record.CurrencyCode).HasMaxLength(3);
        builder.Property(record => record.Status).HasMaxLength(40).IsRequired();
        builder.Property(record => record.BatchNumber).HasMaxLength(100);
        builder.Property(record => record.SerialNumber).HasMaxLength(150);
        builder.Property(record => record.Quantity).HasPrecision(19,4);
        builder.Property(record => record.ReservedQuantity).HasPrecision(19,4);
        builder.Property(record => record.UnitCost).HasPrecision(19,4);
        builder.Property(record => record.MinimumStockLevel).HasPrecision(19,4);
        builder.Property(record => record.MetadataJson).HasColumnType("nvarchar(max)");
        builder.HasIndex(record => new { record.CompanyId,record.Type,record.Code }).IsUnique();
        builder.HasIndex(record => new { record.CompanyId,record.Type,record.Status });
        builder.HasIndex(record => new { record.CompanyId,record.ProductId,record.WarehouseId });
        builder.HasIndex(record => new { record.CompanyId,record.ExpirationDateUtc });
    }
}
