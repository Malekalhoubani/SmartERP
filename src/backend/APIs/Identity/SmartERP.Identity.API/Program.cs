using SmartERP.ServiceDefaults;
using SmartERP.ServiceDefaults.FoundationOptions;
using SmartERP.Identity.DataAccess;
using SmartERP.Identity.Services;
using SmartERP.Identity.API.Endpoints;


var builder = WebApplication.CreateBuilder(args);

builder.AddSmartErpFoundation();
builder.Services.AddIdentityDataAccess(builder.Configuration);
builder.Services.AddIdentityServices();

var app = builder.Build();

app.UseSmartErpFoundation();

app.MapSmartErpFoundation(
    "SmartERP Identity API");

app.MapIdentityEndpoints();

app.Run();

public partial class Program
{
}
