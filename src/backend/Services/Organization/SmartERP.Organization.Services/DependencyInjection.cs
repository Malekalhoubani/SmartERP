using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SmartERP.Organization.Services.Implementations;
using SmartERP.Organization.Services.Interfaces;
using SmartERP.Organization.Services.Validators.Company;

namespace SmartERP.Organization.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddOrganizationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<
            CreateCompanyRequestDtoValidator>();

        services.AddScoped<ICompanyService, CompanyService>();

        return services;
    }
}
