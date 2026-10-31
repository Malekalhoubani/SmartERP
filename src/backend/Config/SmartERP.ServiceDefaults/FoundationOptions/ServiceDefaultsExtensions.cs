using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
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
using System.Threading.RateLimiting;

namespace SmartERP.ServiceDefaults.FoundationOptions
{
    public static class ServiceDefaultsExtensions
    {
        public const string WebCorsPolicy = "web";
        public const string AuthenticationRateLimitPolicy ="authentication";
        public const string GatewayRateLimitPolicy = "gateway";


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

                options.OnRejected = WriteRateLimitResponseAsync;

                options.AddPolicy(
                    AuthenticationRateLimitPolicy,
                    httpContext => CreateFixedWindowPartition(
                        httpContext,
                        rateLimitSettings.AuthenticationPermitLimit,
                        rateLimitSettings.AuthenticationWindowSeconds,
                        AuthenticationRateLimitPolicy));

                options.AddPolicy(
                    GatewayRateLimitPolicy,
                    httpContext => CreateFixedWindowPartition(
                        httpContext,
                        rateLimitSettings.GatewayPermitLimit,
                        rateLimitSettings.GatewayWindowSeconds,
                        GatewayRateLimitPolicy));
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

            app.UseStatusCodePages(WriteStatusCodeResponseAsync);

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
                .Validate(
                    settings => settings.GatewayPermitLimit > 0,
                    "Gateway permit limit must be positive.")
                .Validate(
                    settings => settings.GatewayWindowSeconds > 0,
                    "Gateway window must be positive.")
                .ValidateOnStart();
        }

        private static RateLimitPartition<string> CreateFixedWindowPartition(
            HttpContext httpContext,
            int permitLimit,
            int windowSeconds,
            string policyName)
        {
            var clientAddress =
                httpContext.Connection.RemoteIpAddress?.ToString()
                ?? "unknown";

            return RateLimitPartition.GetFixedWindowLimiter(
                $"{policyName}:{clientAddress}",
                _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = permitLimit,
                    Window = TimeSpan.FromSeconds(windowSeconds),
                    QueueLimit = 0,
                    QueueProcessingOrder =
                        QueueProcessingOrder.OldestFirst,
                    AutoReplenishment = true
                });
        }

        private static async ValueTask WriteRateLimitResponseAsync(
            OnRejectedContext rejectedContext,
            CancellationToken cancellationToken)
        {
            var httpContext = rejectedContext.HttpContext;

            httpContext.Response.StatusCode =
                StatusCodes.Status429TooManyRequests;

            if (rejectedContext.Lease.TryGetMetadata(
                    MetadataName.RetryAfter,
                    out var retryAfter))
            {
                httpContext.Response.Headers.RetryAfter =
                    Math.Ceiling(retryAfter.TotalSeconds).ToString();
            }

            await WriteProblemDetailsAsync(
                httpContext,
                StatusCodes.Status429TooManyRequests,
                "Too many requests",
                "Too many requests were received. Please try again later.",
                cancellationToken);
        }

        private static Task WriteStatusCodeResponseAsync(
            StatusCodeContext statusCodeContext)
        {
            var httpContext = statusCodeContext.HttpContext;
            var (title, detail) = httpContext.Response.StatusCode switch
            {
                StatusCodes.Status401Unauthorized =>
                    ("Unauthorized", "Authentication is required to access this resource."),

                StatusCodes.Status403Forbidden =>
                    ("Forbidden", "You do not have permission to access this resource."),

                StatusCodes.Status404NotFound =>
                    ("Resource not found", "The requested resource could not be found."),

                StatusCodes.Status405MethodNotAllowed =>
                    ("Method not allowed", "The HTTP method is not supported for this resource."),

                StatusCodes.Status429TooManyRequests =>
                    ("Too many requests", "Too many requests were received. Please try again later."),

                _ =>
                    ("Request failed", "The request could not be completed.")
            };

            return WriteProblemDetailsAsync(
                    httpContext,
                    httpContext.Response.StatusCode,
                    title,
                    detail,
                    httpContext.RequestAborted)
                .AsTask();
        }

        private static async ValueTask WriteProblemDetailsAsync(
            HttpContext httpContext,
            int statusCode,
            string title,
            string detail,
            CancellationToken cancellationToken)
        {
            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = httpContext.Request.Path
            };

            problemDetails.Extensions["correlationId"] =
                httpContext.TraceIdentifier;

            problemDetails.Extensions["timestamp"] =
                DateTimeOffset.UtcNow;

            var problemDetailsService =
                httpContext.RequestServices
                    .GetRequiredService<IProblemDetailsService>();

            await problemDetailsService.TryWriteAsync(
                new ProblemDetailsContext
                {
                    HttpContext = httpContext,
                    ProblemDetails = problemDetails
                });
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
