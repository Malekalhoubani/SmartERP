using Microsoft.EntityFrameworkCore;
using SmartERP.Sales.Domain.Entities;
namespace SmartERP.Sales.DataAccess.Contexts;
public sealed class SalesDbContext(DbContextOptions<SalesDbContext> options) : DbContext(options) { public DbSet<SalesFinancialRecord> Records => Set<SalesFinancialRecord>(); protected override void OnModelCreating(ModelBuilder builder) => builder.ApplyConfigurationsFromAssembly(typeof(SalesDbContext).Assembly); }
