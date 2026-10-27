namespace SmartERP.Identity.API.Services;

public sealed class FrontendOriginResolver(
    IConfiguration configuration)
{
    public string? Resolve(HttpRequest request)
    {
        return Resolve(request.Headers.Origin.FirstOrDefault());
    }

    public string? Resolve(string? origin)
    {
        var requestOrigin = origin?.Trim().TrimEnd('/');

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
