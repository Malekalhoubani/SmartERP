using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SmartERP.ServiceDefaults.FoundationOptions
{
    public static class ServiceDefaultsExtensions
    {
        public const string WebCorsPolicy = "web";
        public const string AuthenticationRateLimitPolicy ="authentication";


        public static WebApplicationBuilder AddSmartErpFoundation (this WebApplicationBuilder builder)
        {
            RegisterOptions(builder);

            var corsSettings = builder.Configuration.GetSection(CorsSettings.SectionName).Get<CorsSettings>()?? new CorsSettings();

            var rateLimitSettings = builder.Configuration.GetSection(RateLimitSettings.SectionName).Get<RateLimitSettings>() ?? new RateLimitSettings();


            builder.Services.AddProblemDetails();
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

            builder.Services.AddHealthChecks();

            builder.Services.AddHttpContextAccessor();


            builder.Services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(WebCorsPolicy, policy =>
               {
                   policy
                       .WithOrigins(
                           corsSettings.AllowedOrigins)
                       .AllowAnyHeader()
                       .AllowAnyMethod()
                       .AllowCredentials()
                       .SetPreflightMaxAge(
                           TimeSpan.FromMinutes(10));
               });
            });


            builder.Services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode =
                    StatusCodes.Status429TooManyRequests;

                options.AddFixedWindowLimiter(
                    AuthenticationRateLimitPolicy,
                    limiter =>
                    {
                        limiter.PermitLimit =
                            rateLimitSettings
                                .AuthenticationPermitLimit;

                        limiter.Window =
                            TimeSpan.FromSeconds(
                                rateLimitSettings
                                    .AuthenticationWindowSeconds);

                        limiter.QueueLimit = 0;

                        limiter.AutoReplenishment = true;
                    });
            });

            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
              options.ForwardedHeaders =
                  ForwardedHeaders.XForwardedFor |
                  ForwardedHeaders.XForwardedProto;
            });

            return builder;
        }

        public static WebApplication UseSmartErpFoundation(this WebApplication app)
        {
            app.UseForwardedHeaders();

            app.UseMiddleware<CorrelationIdMiddleware>();

            app.UseExceptionHandler();

            app.UseStatusCodePages();

            if (!app.Environment.IsDevelopment())
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseMiddleware<SecurityHeadersMiddleware>();

            app.UseCors(WebCorsPolicy);

            app.UseRateLimiter();

            return app;
        }
        private static void RegisterOptions(WebApplicationBuilder builder)
        {
            builder.Services
                .AddOptions<CorsSettings>()
                .Bind(
                    builder.Configuration.GetSection(
                        CorsSettings.SectionName))
                .Validate(
                    settings =>
                        settings.AllowedOrigins is { Length: > 0 },
                    "At least one CORS origin is required.")
                .ValidateOnStart();

            builder.Services
                .AddOptions<RateLimitSettings>()
                .Bind(
                    builder.Configuration.GetSection(
                        RateLimitSettings.SectionName))
                .Validate(
                    settings =>
                        settings.AuthenticationPermitLimit > 0,
                    "Authentication permit limit must be positive.")
                .Validate(
                    settings =>
                        settings.AuthenticationWindowSeconds > 0,
                    "Authentication window must be positive.")
                .ValidateOnStart();
        }

        public static WebApplication MapSmartErpFoundation(this WebApplication app,string serviceName)
        {
            app.MapGet("/", () =>
            {
                return Results.Ok(new
                {
                    service = serviceName,
                    status = "running",
                    utcTime = DateTimeOffset.UtcNow
                });
            });

            app.MapHealthChecks(
                "/health/live",
                new HealthCheckOptions
                {
                    Predicate = _ => false
                });

            app.MapHealthChecks(
                "/health/ready",
                new HealthCheckOptions
                {
                    Predicate = healthCheck =>
                        healthCheck.Tags.Contains("ready"),

                    ResponseWriter = WriteHealthResponseAsync
                });

            return app;
        }

        private static Task WriteHealthResponseAsync(HttpContext context,HealthReport healthReport)
        {
            context.Response.ContentType ="application/json";

            var response = new
            {
                status = healthReport.Status.ToString(),

                durationInMilliseconds =
                    healthReport.TotalDuration.TotalMilliseconds,

                checks = healthReport.Entries.Select(entry => new
                {
                    name = entry.Key,

                    status =
                        entry.Value.Status.ToString(),

                    durationInMilliseconds =
                        entry.Value.Duration.TotalMilliseconds,

                    description =
                        entry.Value.Description
                })
            };

            return context.Response.WriteAsync(
                JsonSerializer.Serialize(response));
        }
    }
}
