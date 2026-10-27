using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SmartERP.Identity.Services.Features.Authentication.Login;
using SmartERP.Identity.Services.Features.Authentication.Register;
using SmartERP.Identity.Services.Features.Authorization.Implementations;
using SmartERP.Identity.Services.Features.Authentication.ForgotPassword.Implementations;
using SmartERP.Identity.Services.Interfaces.Authentication;
using SmartERP.Identity.Services.Interfaces.Authorization;

namespace SmartERP.Identity.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddScoped<IRegisterService, RegisterService>();
        services.AddScoped<ILoginService, LoginService>();
        services.AddScoped<IForgotPasswordService, ForgotPasswordService>();
        services.AddScoped<IRolePermissionService, RolePermissionService>();

        return services;
    }
}
