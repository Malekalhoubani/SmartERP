using SmartERP.ServiceDefaults;
using SmartERP.ServiceDefaults.FoundationOptions;

var builder = WebApplication.CreateBuilder(args);

builder.AddSmartErpFoundation();

var app = builder.Build();

app.UseSmartErpFoundation();

app.MapSmartErpFoundation(
    "SmartERP Identity API");

app.Run();

public partial class Program
{
}
