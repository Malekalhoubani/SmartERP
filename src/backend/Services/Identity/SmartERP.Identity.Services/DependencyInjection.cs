using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SmartERP.Identity.Services.Features.Authentication.Login;
using SmartERP.Identity.Services.Features.Authentication.Register;

namespace SmartERP.Identity.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddScoped<IRegisterService, RegisterService>();
        services.AddScoped<ILoginService, LoginService>();

        return services;
    }
}
