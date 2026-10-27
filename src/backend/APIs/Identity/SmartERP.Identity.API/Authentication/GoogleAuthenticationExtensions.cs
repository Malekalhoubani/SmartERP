using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace SmartERP.Identity.API.Authentication;

public static class GoogleAuthenticationExtensions
{
    public static IServiceCollection AddGoogleExternalAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddAuthentication()
            .AddCookie(
                IdentityConstants.ExternalScheme,
                options =>
                {
                    options.Cookie.Name = "SmartERP.ExternalLogin";
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.Cookie.SameSite = SameSiteMode.Lax;
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                    options.SlidingExpiration = false;
                });

        var clientId = configuration[
            "Authentication:Google:ClientId"];

        var clientSecret = configuration[
            "Authentication:Google:ClientSecret"];

        if (string.IsNullOrWhiteSpace(clientId) ||
            string.IsNullOrWhiteSpace(clientSecret))
        {
            return services;
        }

        services
            .AddAuthentication()
            .AddGoogle(
                GoogleDefaults.AuthenticationScheme,
                options =>
                {
                    options.ClientId = clientId;
                    options.ClientSecret = clientSecret;
                    options.SignInScheme = IdentityConstants.ExternalScheme;
                    options.CallbackPath = "/api/auth/google/signin-callback";
                    options.SaveTokens = false;

                    options.CorrelationCookie.SameSite = SameSiteMode.None;
                    options.CorrelationCookie.SecurePolicy =
                        CookieSecurePolicy.Always;

                    options.Events.OnCreatingTicket = context =>
                    {
                        if (context.User.TryGetProperty(
                                "verified_email",
                                out var verifiedEmail) &&
                            verifiedEmail.ValueKind ==
                                System.Text.Json.JsonValueKind.True)
                        {
                            context.Identity?.AddClaim(
                                new Claim(
                                    "google_email_verified",
                                    "true"));
                        }

                        return Task.CompletedTask;
                    };
                });

        return services;
    }
}
