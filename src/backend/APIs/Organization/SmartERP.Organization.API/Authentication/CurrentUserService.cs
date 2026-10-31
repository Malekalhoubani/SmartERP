using SmartERP.Organization.Services.Interfaces;

namespace SmartERP.Organization.API.Authentication;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService( IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int UserId
    {
        get
        {
            var httpContext =
                _httpContextAccessor.HttpContext
                ?? throw new UnauthorizedAccessException(
                    "HTTP context is not available.");

            if (httpContext.User.Identity?.IsAuthenticated != true)
            {
                throw new UnauthorizedAccessException(
                    "The user is not authenticated.");
            }

            var userIdValue =
                httpContext.User.FindFirst("sub")?.Value;

            if (!int.TryParse(userIdValue, out var userId) ||
                userId <= 0)
            {
                throw new UnauthorizedAccessException(
                    "The authenticated user identifier is invalid.");
            }

            return userId;
        }
    }
}