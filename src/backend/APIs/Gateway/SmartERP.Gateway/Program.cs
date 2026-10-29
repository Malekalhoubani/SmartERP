using SmartERP.ServiceDefaults;
using SmartERP.ServiceDefaults.FoundationOptions;
using SmartERP.Gateway.Authentication;
using SmartERP.Gateway.ErrorHandling;

var builder = WebApplication.CreateBuilder(args);

builder.AddSmartErpFoundation();
builder.AddGatewayAuthentication();

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(
        builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.UseSmartErpFoundation();

app.UseAuthentication();
app.UseAuthorization();

app.MapSmartErpFoundation(
    "SmartERP API Gateway");

app.MapGatewayReverseProxy();

app.Run();

public partial class Program
{
}
