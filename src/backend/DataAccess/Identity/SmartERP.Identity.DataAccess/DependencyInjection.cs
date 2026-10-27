using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartERP.Identity.DataAccess.Contexts;
using SmartERP.Identity.DataAccess.Options;
using SmartERP.Identity.DataAccess.Repositories;
using SmartERP.Identity.DataAccess.Services;
using SmartERP.Identity.Domain.Entities;
using SmartERP.Identity.Repositories.Interfaces;
using SmartERP.Identity.Services.Interfaces;

namespace SmartERP.Identity.DataAccess;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityDataAccess(this IServiceCollection services,IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("IdentityDatabase")
            ?? throw new InvalidOperationException(
                "Connection string 'IdentityDatabase' was not found.");

        services.AddDbContext<IdentityApplicationDbContext>(options =>
        {
            options.UseSqlServer(
                connectionString,
                sqlServerOptions =>
                {
                    sqlServerOptions.MigrationsAssembly(
                        typeof(IdentityApplicationDbContext)
                            .Assembly.FullName);

                    sqlServerOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null);
                });
        });
        services.AddAuthentication();

        services.AddDataProtection();

        services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;

                options.Password.RequiredLength = 12;
                options.Password.RequiredUniqueChars = 1;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Lockout.AllowedForNewUsers = true;

                options.Lockout.MaxFailedAccessAttempts = 5;

                options.Lockout.DefaultLockoutTimeSpan =
                    TimeSpan.FromMinutes(15);

                options.SignIn.RequireConfirmedEmail = true;
            })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<IdentityApplicationDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        services.AddScoped<IIdentityRepository, IdentityRepository>();

        services
            .AddOptions<EmailOptions>()
            .Bind(configuration.GetSection(EmailOptions.SectionName))
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.SmtpHost),
                "Email:SmtpHost is required.")
            .Validate(
                options => options.SmtpPort is > 0 and <= 65535,
                "Email:SmtpPort is invalid.")
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.SenderEmail),
                "Email:SenderEmail is required.")
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.Username),
                "Email:Username is required.")
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.Password),
                "Email:Password is required.")
            .ValidateOnStart();




        services.Configure<DataProtectionTokenProviderOptions>(options =>
        {
            options.TokenLifespan = TimeSpan.FromHours(24);
        });

        services.AddScoped<IEmailService, EmailService>();

        return services;
    }
}
