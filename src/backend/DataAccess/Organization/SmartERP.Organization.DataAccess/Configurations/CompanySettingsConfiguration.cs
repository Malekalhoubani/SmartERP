using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartERP.Organization.DataAccess.Configurations.Common;
using SmartERP.Organization.Domain.Entities;

namespace SmartERP.Organization.DataAccess.Configurations;

public sealed class CompanySettingsConfiguration
    : IEntityTypeConfiguration<CompanySettings>
{
    public void Configure(
        EntityTypeBuilder<CompanySettings> builder)
    {
        builder.ToTable(
            "CompanySettings",
            "organization");

        builder.ConfigureAuditableEntity();

        builder.Property(settings =>
                settings.DefaultLanguage)
            .HasMaxLength(5)
            .IsUnicode(false)
            .HasDefaultValue("en")
            .IsRequired();

        builder.Property(settings =>
                settings.TimeZoneId)
            .HasMaxLength(100)
            .HasDefaultValue("Asia/Amman")
            .IsRequired();

        builder.Property(settings =>
                settings.DateFormat)
            .HasMaxLength(30)
            .IsUnicode(false)
            .HasDefaultValue("yyyy-MM-dd")
            .IsRequired();

        builder.Property(settings =>
                settings.TimeFormat)
            .HasMaxLength(20)
            .IsUnicode(false)
            .HasDefaultValue("HH:mm")
            .IsRequired();

        builder.Property(settings =>
                settings.NumberDecimalPlaces)
            .HasDefaultValue(2)
            .IsRequired();

        builder.Property(settings =>
                settings.QuantityDecimalPlaces)
            .HasDefaultValue(3)
            .IsRequired();

        builder.Property(settings =>
                settings.SalesInvoicePrefix)
            .HasMaxLength(20)
            .IsUnicode(false)
            .HasDefaultValue("INV")
            .IsRequired();

        builder.Property(settings =>
                settings.PurchaseOrderPrefix)
            .HasMaxLength(20)
            .IsUnicode(false)
            .HasDefaultValue("PO")
            .IsRequired();

        builder.Property(settings => settings.LogoUrl)
            .HasMaxLength(2048);

        builder.HasOne(settings => settings.Company)
            .WithOne(company => company.Settings)
            .HasForeignKey<CompanySettings>(
                settings => settings.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(settings => settings.CompanyId)
            .IsUnique();
    }
}