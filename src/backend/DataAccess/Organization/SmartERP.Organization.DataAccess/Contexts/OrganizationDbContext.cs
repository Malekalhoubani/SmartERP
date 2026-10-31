using Microsoft.EntityFrameworkCore;
using SmartERP.Organization.Domain.Entities;

namespace SmartERP.Organization.DataAccess.Contexts;

public sealed class OrganizationDbContext : DbContext
{
    public OrganizationDbContext(DbContextOptions<OrganizationDbContext> options): base(options)
    {
    }

    public DbSet<Company> Companies =>Set<Company>();

    public DbSet<CompanyMembership> CompanyMemberships =>Set<CompanyMembership>();

    public DbSet<Branch> Branches =>Set<Branch>();

    public DbSet<Currency> Currencies =>Set<Currency>();

    public DbSet<CompanyCurrency> CompanyCurrencies =>Set<CompanyCurrency>();

    public DbSet<Tax> Taxes =>Set<Tax>();

    public DbSet<FiscalYear> FiscalYears =>Set<FiscalYear>();

    public DbSet<CompanySettings> CompanySettings => Set<CompanySettings>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(OrganizationDbContext).Assembly);
    }
}