using Microsoft.EntityFrameworkCore;
using SmartERP.Inventory.API.Authentication;
using SmartERP.Inventory.API.Endpoints;
using SmartERP.Inventory.DataAccess;
using SmartERP.Inventory.DataAccess.Contexts;
using SmartERP.Inventory.Services;
using SmartERP.ServiceDefaults;
using SmartERP.ServiceDefaults.FoundationOptions;

var builder = WebApplication.CreateBuilder(args);
builder.AddSmartErpFoundation();
builder.AddInventoryAuthentication();
builder.Services.AddInventoryDataAccess(builder.Configuration);
builder.Services.AddInventoryServices();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IRequestContext,RequestContext>();
builder.Services.AddScoped<ICompanyAccessValidator,CompanyAccessValidator>();
builder.Services.AddHttpClient("OrganizationAccess",client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:OrganizationBaseUrl"]
        ?? throw new InvalidOperationException("Services:OrganizationBaseUrl is required."));
    client.Timeout = TimeSpan.FromSeconds(10);
});

var app = builder.Build();
app.UseSmartErpFoundation();
app.UseAuthentication();
app.UseAuthorization();
app.MapSmartErpFoundation("SmartERP Inventory API");
app.MapCatalogEndpoints();

if (app.Environment.IsDevelopment())
{
    await using var scope = app.Services.CreateAsyncScope();
    await scope.ServiceProvider.GetRequiredService<InventoryDbContext>().Database.MigrateAsync();
}

app.Run();

public partial class Program { }
