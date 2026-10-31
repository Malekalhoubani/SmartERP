using SmartERP.Organization.DataAccess;
using SmartERP.ServiceDefaults;
using SmartERP.ServiceDefaults.FoundationOptions;

var builder = WebApplication.CreateBuilder(args);

builder.AddSmartErpFoundation();

builder.Services.AddOrganizationDataAccess(
    builder.Configuration);

var app = builder.Build();

app.UseSmartErpFoundation();

app.MapSmartErpFoundation("SmartERP Organization API");

app.Run();

public partial class Program
{
}