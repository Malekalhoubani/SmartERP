using Microsoft.AspNetCore.Mvc;
using Yarp.ReverseProxy.Forwarder;

namespace SmartERP.Gateway.ErrorHandling;

public static class GatewayProxyErrorExtensions
{
    public static void MapGatewayReverseProxy(
        this WebApplication app)
    {
        app.MapReverseProxy(proxyPipeline =>
        {
            proxyPipeline.Use(async (httpContext, next) =>
            {
                await next();

                var errorFeature =
                    httpContext.GetForwarderErrorFeature();

                if (errorFeature is null ||
                    httpContext.Response.HasStarted ||
                    httpContext.RequestAborted.IsCancellationRequested)
                {
                    return;
                }

                var isTimeout = errorFeature.Error is
                    ForwarderError.RequestTimedOut or
                    ForwarderError.UpgradeActivityTimeout;

                var statusCode = isTimeout
                    ? StatusCodes.Status504GatewayTimeout
                    : StatusCodes.Status502BadGateway;

                var logger = httpContext.RequestServices
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("SmartERP.Gateway.Proxy");

                logger.LogWarning(
                    errorFeature.Exception,
                    "Proxy request failed. Error: {ProxyError}, TraceId: {TraceId}",
                    errorFeature.Error,
                    httpContext.TraceIdentifier);

                httpContext.Response.Clear();
                httpContext.Response.StatusCode = statusCode;

                var problemDetails = new ProblemDetails
                {
                    Status = statusCode,
                    Title = isTimeout
                        ? "Gateway timeout"
                        : "Service unavailable",
                    Detail = isTimeout
                        ? "The downstream service did not respond in time."
                        : "The downstream service is currently unavailable.",
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
                        Exception = errorFeature.Exception,
                        ProblemDetails = problemDetails
                    });
            });
        });
    }
}
