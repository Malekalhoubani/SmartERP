using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartERP.Identity.Domain.Entities;

namespace SmartERP.Identity.DataAccess.Contexts;

public sealed class IdentityApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
{
    public IdentityApplicationDbContext( DbContextOptions<IdentityApplicationDbContext> options): base(options)
    {
    }
    public DbSet<UserSession> UserSessions => Set<UserSession>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<Permission> Permissions => Set<Permission>();

    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(
            typeof(IdentityApplicationDbContext).Assembly);

        builder.Entity<IdentityUserRole<int>>()
            .ToTable("UserRoles", "identity");

        builder.Entity<IdentityUserClaim<int>>()
            .ToTable("UserClaims", "identity");

        builder.Entity<IdentityRoleClaim<int>>()
            .ToTable("RoleClaims", "identity");

        builder.Entity<IdentityUserLogin<int>>()
            .ToTable("UserLogins", "identity");

        builder.Entity<IdentityUserToken<int>>()
            .ToTable("UserTokens", "identity");
    }
}