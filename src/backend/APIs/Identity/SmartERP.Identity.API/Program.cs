using SmartERP.ServiceDefaults;
using SmartERP.ServiceDefaults.FoundationOptions;
using SmartERP.Identity.DataAccess;


var builder = WebApplication.CreateBuilder(args);

builder.AddSmartErpFoundation();
builder.Services.AddIdentityDataAccess(builder.Configuration);

var app = builder.Build();

app.UseSmartErpFoundation();

app.MapSmartErpFoundation(
    "SmartERP Identity API");

app.Run();

public partial class Program
{
}
