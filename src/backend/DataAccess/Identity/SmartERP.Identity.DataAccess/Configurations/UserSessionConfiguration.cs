using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartERP.Identity.Domain.Entities;

namespace SmartERP.Identity.DataAccess.Configurations;

public sealed class UserSessionConfiguration: IEntityTypeConfiguration<UserSession>
{
    public void Configure(EntityTypeBuilder<UserSession> builder)
    {
        builder.ToTable("UserSessions", "identity");

        builder.HasKey(session => session.Id);

        builder.Property(session => session.Id)
            .ValueGeneratedOnAdd();

        builder.Property(session => session.CreatedAtUtc)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        builder.Property(session => session.LastActivityAtUtc)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        builder.Property(session => session.ExpiresAtUtc)
            .IsRequired();

        builder.Property(session => session.RevokedReason)
            .HasMaxLength(250);

        builder.Property(session => session.IpAddress)
            .HasMaxLength(45);

        builder.Property(session => session.UserAgent)
            .HasMaxLength(512);

        builder.Ignore(session => session.IsActive);

        builder.HasIndex(session => session.UserId);

        builder.HasIndex(session => session.ExpiresAtUtc);

        builder.HasOne(session => session.User)
            .WithMany(user => user.Sessions)
            .HasForeignKey(session => session.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}