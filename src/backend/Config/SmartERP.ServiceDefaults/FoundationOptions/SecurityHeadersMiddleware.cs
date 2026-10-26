using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartERP.ServiceDefaults.FoundationOptions
{
    public sealed class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            context.Response.OnStarting(() =>
            {
                context.Response.Headers["X-Content-Type-Options"] =
                    "nosniff";

                context.Response.Headers["X-Frame-Options"] =
                    "DENY";

                context.Response.Headers["Referrer-Policy"] =
                    "no-referrer";

                context.Response.Headers["Permissions-Policy"] =
                    "camera=(), microphone=(), geolocation=()";

                return Task.CompletedTask;
            });

            await _next(context);
        }
    }
}
