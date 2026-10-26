using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartERP.Identity.DataAccess.Contexts;
using SmartERP.Identity.Domain.Entities;

namespace SmartERP.Identity.DataAccess;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityDataAccess(this IServiceCollection services,IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("IdentityDatabase")
            ?? throw new InvalidOperationException(
                "Connection string 'IdentityDatabase' was not found.");

        services.AddDbContext<IdentityApplicationDbContext>(options =>
        {
            options.UseSqlServer(
                connectionString,
                sqlServerOptions =>
                {
                    sqlServerOptions.MigrationsAssembly(
                        typeof(IdentityApplicationDbContext)
                            .Assembly.FullName);

                    sqlServerOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null);
                });
        });
        services.AddAuthentication();

        services.AddDataProtection();

        services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<IdentityApplicationDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        return services;
    }
}
