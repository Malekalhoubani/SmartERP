using Microsoft.EntityFrameworkCore;
using SmartERP.Inventory.Domain.Entities;

namespace SmartERP.Inventory.DataAccess.Contexts;

public sealed class InventoryDbContext(DbContextOptions<InventoryDbContext> options) : DbContext(options)
{
    public DbSet<CatalogItem> CatalogItems => Set<CatalogItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InventoryDbContext).Assembly);
    }
}
