using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartERP.Identity.DataAccess.Contexts;
using SmartERP.Identity.DataAccess.Options;
using SmartERP.Identity.DataAccess.Repositories;
using SmartERP.Identity.DataAccess.Services;
using SmartERP.Identity.Domain.Entities;
using SmartERP.Identity.Repositories.Interfaces;
using SmartERP.Identity.Services.Interfaces;
using SmartERP.Identity.Services.Features.Authentication.Sessions;
using SmartERP.Identity.DataAccess.Seeding;

namespace SmartERP.Identity.DataAccess;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityDataAccess(this IServiceCollection services,IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("IdentityDatabase")
            ?? throw new InvalidOperationException(
                "Connection string 'IdentityDatabase' was not found.");

        var jwtSection =
            configuration.GetRequiredSection(JwtOptions.SectionName);

        var jwtOptions = jwtSection.Get<JwtOptions>()
            ?? throw new InvalidOperationException(
                "JWT configuration was not found.");

        var signingKeyBytes =
            GetJwtSigningKey(jwtOptions.SigningKey);

        var refreshTokenSection =
            configuration.GetRequiredSection(
                RefreshTokenOptions.SectionName);

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
        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;

                options.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.SaveToken = false;

                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey =
                            new SymmetricSecurityKey(signingKeyBytes),

                        ValidateIssuer = true,
                        ValidIssuer = jwtOptions.Issuer,

                        ValidateAudience = true,
                        ValidAudience = jwtOptions.Audience,

                        ValidateLifetime = true,
                        RequireExpirationTime = true,
                        RequireSignedTokens = true,

                        NameClaimType = JwtRegisteredClaimNames.Name,
                        RoleClaimType = "role",

                        ValidAlgorithms =
                        [
                            SecurityAlgorithms.HmacSha256
                        ],

                        ClockSkew = TimeSpan.FromSeconds(30)
                    };
            });

        services.AddAuthorization();

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
        services.AddScoped< IRolePermissionRepository,RolePermissionRepository>();
        services.AddScoped<
            IAuthenticationSessionRepository,
            AuthenticationSessionRepository>();

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

        services
            .AddOptions<JwtOptions>()
            .Bind(jwtSection)
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.Issuer),
                "Jwt:Issuer is required.")
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.Audience),
                "Jwt:Audience is required.")
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.SigningKey),
                "Jwt:SigningKey is required.")
            .Validate(
                options =>
                    options.AccessTokenExpirationMinutes is > 0 and <= 60,
                "JWT expiration must be between 1 and 60 minutes.")
            .ValidateOnStart();

        services
            .AddOptions<RefreshTokenOptions>()
            .Bind(refreshTokenSection)
            .Validate(
                options => options.ExpirationDays is > 0 and <= 90,
                "RefreshToken:ExpirationDays must be between 1 and 90.")
            .Validate(
                options => options.TokenSizeInBytes is >= 32 and <= 128,
                "RefreshToken:TokenSizeInBytes must be between 32 and 128.")
            .Validate(
                options => options.MaxActiveSessionsPerUser is > 0 and <= 20,
                "RefreshToken:MaxActiveSessionsPerUser must be between 1 and 20.")
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.CookieName),
                "RefreshToken:CookieName is required.")
            .ValidateOnStart();

        services.Configure<DataProtectionTokenProviderOptions>(options =>
        {
            options.TokenLifespan = TimeSpan.FromHours(1);
        });

        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<
            IAuthenticationSessionService,
            AuthenticationSessionService>();
        services.AddSingleton<ITokenService, JwtTokenService>();
        services.AddHostedService<IdentityAuthorizationSeeder>();

        return services;
    }

    private static byte[] GetJwtSigningKey(string signingKey)
    {
        if (string.IsNullOrWhiteSpace(signingKey))
        {
            throw new InvalidOperationException(
                "Jwt:SigningKey is required.");
        }

        try
        {
            var keyBytes = Convert.FromBase64String(signingKey);

            if (keyBytes.Length < 32)
            {
                throw new InvalidOperationException(
                    "JWT signing key must contain at least 256 bits.");
            }

            return keyBytes;
        }
        catch (FormatException exception)
        {
            throw new InvalidOperationException(
                "Jwt:SigningKey must be a valid Base64 value.",
                exception);
        }
    }
}
