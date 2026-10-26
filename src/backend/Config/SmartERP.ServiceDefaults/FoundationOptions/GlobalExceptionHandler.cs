using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SmartERP.ServiceDefaults;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(IProblemDetailsService problemDetailsService,ILogger<GlobalExceptionHandler> logger)
    {
        _problemDetailsService = problemDetailsService;
        _logger = logger;
    }

          public async ValueTask<bool> TryHandleAsync(HttpContext httpContext,Exception exception,CancellationToken cancellationToken)
    {
        var (statusCode, title, detail) = exception switch
        {
            ArgumentException => (StatusCodes.Status400BadRequest,"Bad Request",exception.Message),

            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized,"Unauthorized","You are not authorized to perform this operation."),

            KeyNotFoundException => (StatusCodes.Status404NotFound,"Resource not found",exception.Message),

            InvalidOperationException => (StatusCodes.Status409Conflict,"Operation conflict",exception.Message),

            _ => (
                StatusCodes.Status500InternalServerError,
                "Unexpected error",
                "An unexpected error occurred.")
        };

        LogException(exception,statusCode,httpContext.TraceIdentifier);

        httpContext.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        problemDetails.Extensions["correlationId"] = httpContext.TraceIdentifier;

        problemDetails.Extensions["timestamp"] = DateTimeOffset.UtcNow;

        return await _problemDetailsService.TryWriteAsync(
            new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = problemDetails
            });
    }

    private void LogException(Exception exception,int statusCode,string traceId)
    {
        if (statusCode >= 500)
        {
            _logger.LogError(
                exception,
                "Unexpected request error. StatusCode: {StatusCode}, TraceId: {TraceId}",
                statusCode,
                traceId);

            return;
        }

        _logger.LogWarning(
            exception,
            "Request rejected. StatusCode: {StatusCode}, TraceId: {TraceId}",
            statusCode,
            traceId);
    }
}