using Microsoft.Extensions.DependencyInjection;
using SmartERP.Inventory.Services.Implementations;
using SmartERP.Inventory.Services.Interfaces;

namespace SmartERP.Inventory.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddInventoryServices(this IServiceCollection services)
    {
        services.AddScoped<ICatalogService,CatalogService>();
        return services;
    }
}
