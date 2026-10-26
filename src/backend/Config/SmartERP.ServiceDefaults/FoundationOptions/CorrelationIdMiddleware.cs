using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartERP.ServiceDefaults.FoundationOptions
{
     public sealed class CorrelationIdMiddleware(RequestDelegate next)
    {
        public const string HeaderName = "X-Correlation-ID";
        public async Task InvokeAsync(HttpContext context)
        {
            var incomingCorrelationId = context.Request.Headers[HeaderName].FirstOrDefault();
            var correlationId = IsValid(incomingCorrelationId)?incomingCorrelationId!: Guid.NewGuid().ToString("N");

            context.TraceIdentifier=correlationId;

            Activity.Current?.SetTag("correlation.id", correlationId);


            context.Response.OnStarting(() =>
            {
                context.Response.Headers[HeaderName] = correlationId;
                return Task.CompletedTask;
            });

            await next(context);
        }

        private static bool IsValid(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            if (value.Length > 64)
                return false;

            return value.All(character =>
                char.IsLetterOrDigit(character) ||
                character is '-' or '_');
        }

    }
}
