using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SmartERP.Identity.DataAccess.Contexts;

public sealed class IdentityApplicationDbContextFactory
    : IDesignTimeDbContextFactory<IdentityApplicationDbContext>
{
    public IdentityApplicationDbContext CreateDbContext(string[] args)
    {
        var settingsDirectory = FindSettingsDirectory();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(settingsDirectory)
            .AddJsonFile(
                "appsettings.json",
                optional: false)
            .AddJsonFile(
                "appsettings.Development.json",
                optional: true)
            .Build();

        var connectionString =
            configuration.GetConnectionString("IdentityDatabase")
            ?? throw new InvalidOperationException(
                "Connection string 'IdentityDatabase' was not found.");

        var optionsBuilder =
            new DbContextOptionsBuilder<IdentityApplicationDbContext>();

        optionsBuilder.UseSqlServer(
            connectionString,
            sqlServerOptions =>
            {
                sqlServerOptions.MigrationsAssembly(
                    typeof(IdentityApplicationDbContext)
                        .Assembly.FullName);
            });

        return new IdentityApplicationDbContext(
            optionsBuilder.Options);
    }

    private static string FindSettingsDirectory()
    {
        var currentDirectory = Directory.GetCurrentDirectory();

        if (File.Exists(
                Path.Combine(currentDirectory, "appsettings.json")))
        {
            return currentDirectory;
        }

        var apiDirectory = Path.Combine(
            currentDirectory,
            "src",
            "backend",
            "APIs",
            "Identity",
            "SmartERP.Identity.API");

        if (File.Exists(
                Path.Combine(apiDirectory, "appsettings.json")))
        {
            return apiDirectory;
        }

        throw new DirectoryNotFoundException(
            "Identity API settings directory was not found.");
    }
}
