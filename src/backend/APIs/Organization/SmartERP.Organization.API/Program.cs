using SmartERP.Organization.API.Authentication;
using SmartERP.Organization.API.Endpoints;
using SmartERP.Organization.DataAccess;
using SmartERP.Organization.Services;
using SmartERP.Organization.Services.Interfaces;
using SmartERP.ServiceDefaults;
using SmartERP.ServiceDefaults.FoundationOptions;

var builder = WebApplication.CreateBuilder(args);

builder.AddSmartErpFoundation();

builder.AddOrganizationAuthentication();

builder.Services.AddOrganizationDataAccess(
    builder.Configuration);

builder.Services.AddOrganizationServices();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ICurrentUserService,CurrentUserService>();

var app = builder.Build();

app.UseSmartErpFoundation();

app.UseAuthentication();

app.UseAuthorization();

app.MapSmartErpFoundation(
    "SmartERP Organization API");

app.MapCompanyEndpoints();

app.Run();

public partial class Program
{
}
