using Microsoft.Extensions.DependencyInjection;using SmartERP.Sales.Services.Implementations;using SmartERP.Sales.Services.Interfaces;
namespace SmartERP.Sales.Services;public static class DependencyInjection{public static IServiceCollection AddSalesServices(this IServiceCollection s){s.AddScoped<ISalesFinancialRecordService,SalesFinancialRecordService>();return s;}}
