using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartERP.ServiceDefaults.FoundationOptions
{
     public sealed class CorsSettings
    {
        public const string SectionName = "Cors";
        public string [] AllowedOrigins { get; init; } = ["http://localhost:4200"];
    }

    public sealed class RateLimitSettings
    {
        public const string SectionName = "RateLimiting";
        public int AuthenticationPermitLimit { get; init; } = 5;
        public int AuthenticationWindowSeconds { get; init; } = 60;

    }
}
