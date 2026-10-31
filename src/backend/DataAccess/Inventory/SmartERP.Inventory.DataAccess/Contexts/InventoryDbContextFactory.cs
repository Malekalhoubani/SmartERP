using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SmartERP.Inventory.DataAccess.Contexts;

public sealed class InventoryDbContextFactory : IDesignTimeDbContextFactory<InventoryDbContext>
{
    public InventoryDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("SMARTERP_INVENTORY_CONNECTION")
            ?? "Server=localhost;Database=SmartERPInventoryDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True";
        var options = new DbContextOptionsBuilder<InventoryDbContext>()
            .UseSqlServer(connectionString,sql => sql.MigrationsAssembly(typeof(InventoryDbContext).Assembly.FullName))
            .Options;
        return new InventoryDbContext(options);
    }
}
