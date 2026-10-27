namespace SmartERP.Identity.API.Services;

public sealed class FrontendOriginResolver(
    IConfiguration configuration)
{
    public string? Resolve(HttpRequest request)
    {
        var requestOrigin = request.Headers.Origin
            .FirstOrDefault()?
            .Trim()
            .TrimEnd('/');

        if (string.IsNullOrWhiteSpace(requestOrigin))
        {
            return null;
        }

        var allowedOrigins = configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>()
            ?? [];

        var isAllowed = allowedOrigins.Any(allowedOrigin =>
            string.Equals(
                allowedOrigin.TrimEnd('/'),
                requestOrigin,
                StringComparison.OrdinalIgnoreCase));

        return isAllowed
            ? requestOrigin
            : null;
    }
}
