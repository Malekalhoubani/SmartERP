using SmartERP.ServiceDefaults;
using SmartERP.ServiceDefaults.FoundationOptions;
using SmartERP.Identity.DataAccess;
using SmartERP.Identity.Services;
using SmartERP.Identity.API.Endpoints;
using SmartERP.Identity.API.Authorization;
using SmartERP.Identity.API.Services;
using SmartERP.Identity.API.Authentication;


var builder = WebApplication.CreateBuilder(args);

builder.AddSmartErpFoundation();
builder.Services.AddIdentityDataAccess(builder.Configuration);
builder.Services.AddIdentityServices();
builder.Services.AddPermissionAuthorization();
builder.Services.AddScoped<FrontendOriginResolver>();
builder.Services.AddGoogleExternalAuthentication(builder.Configuration);

var app = builder.Build();

app.UseSmartErpFoundation();

app.UseAuthentication();
app.UseAuthorization();

app.MapSmartErpFoundation(
    "SmartERP Identity API");

app.MapIdentityEndpoints();
app.MapAuthorizationEndpoints();

app.Run();

public partial class Program
{
}
