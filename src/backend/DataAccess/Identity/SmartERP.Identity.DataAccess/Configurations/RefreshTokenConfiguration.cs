using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartERP.Identity.Domain.Entities;

namespace SmartERP.Identity.DataAccess.Configurations;

public sealed class RefreshTokenConfiguration: IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens", "identity");

        builder.HasKey(refreshToken => refreshToken.Id);

        builder.Property(refreshToken => refreshToken.Id)
            .ValueGeneratedOnAdd();

        builder.Property(refreshToken => refreshToken.TokenHash)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(refreshToken => refreshToken.CreatedAtUtc)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        builder.Property(refreshToken => refreshToken.ExpiresAtUtc)
            .IsRequired();

        builder.Property(refreshToken => refreshToken.RevokedReason)
            .HasMaxLength(250);

        builder.Property(refreshToken => refreshToken.ReplacedByTokenHash)
            .HasMaxLength(64);

        builder.Property(refreshToken => refreshToken.CreatedByIpAddress)
            .HasMaxLength(45);

        builder.Property(refreshToken => refreshToken.RevokedByIpAddress)
            .HasMaxLength(45);

        builder.Ignore(refreshToken => refreshToken.IsActive);

        builder.HasIndex(refreshToken => refreshToken.TokenHash)
            .IsUnique();

        builder.HasIndex(refreshToken => refreshToken.UserSessionId);

        builder.HasIndex(refreshToken => refreshToken.ExpiresAtUtc);

        builder.HasOne(refreshToken => refreshToken.UserSession)
            .WithMany(session => session.RefreshTokens)
            .HasForeignKey(refreshToken => refreshToken.UserSessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}