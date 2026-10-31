using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartERP.Inventory.DataAccess.Contexts;
using SmartERP.Inventory.DataAccess.Repositories;
using SmartERP.Inventory.Repositories.Interfaces;

namespace SmartERP.Inventory.DataAccess;

public static class DependencyInjection
{
    public static IServiceCollection AddInventoryDataAccess(this IServiceCollection services,IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("InventoryDatabase")
            ?? throw new InvalidOperationException("Connection string 'InventoryDatabase' was not found.");
        services.AddDbContext<InventoryDbContext>(options => options.UseSqlServer(connectionString,sql =>
        {
            sql.MigrationsAssembly(typeof(InventoryDbContext).Assembly.FullName);
            sql.EnableRetryOnFailure(5,TimeSpan.FromSeconds(10),null);
        }));
        services.AddScoped<ICatalogRepository,CatalogRepository>();
        return services;
    }
}
