using System.Security.Claims;
using SmartERP.Identity.Services.Features.Authentication.Login.Models;

namespace SmartERP.Identity.Services.Interfaces.Authentication;

public interface IGoogleAuthenticationService
{
    Task<LoginResult> AuthenticateAsync(
        ClaimsPrincipal externalPrincipal,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken = default);
}
