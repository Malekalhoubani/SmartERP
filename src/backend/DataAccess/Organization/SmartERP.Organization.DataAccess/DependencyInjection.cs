using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartERP.Organization.DataAccess.Contexts;
using SmartERP.Organization.DataAccess.Repositories;
using SmartERP.Organization.Repositories.Interfaces;

namespace SmartERP.Organization.DataAccess;

public static class DependencyInjection
{
    public static IServiceCollection AddOrganizationDataAccess(this IServiceCollection services,IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString(
                "OrganizationDatabase")
            ?? throw new InvalidOperationException(
                "Connection string " +
                "'OrganizationDatabase' was not found.");

        services.AddDbContext<OrganizationDbContext>(options =>
        {
            options.UseSqlServer(
                connectionString,
                sqlServerOptions =>
                {
                    sqlServerOptions.MigrationsAssembly(
                        typeof(OrganizationDbContext)
                            .Assembly.FullName);

                    sqlServerOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null);
                });
        });

        services.AddScoped<ICompanyRepository, CompanyRepository>();

        return services;
    }
}
