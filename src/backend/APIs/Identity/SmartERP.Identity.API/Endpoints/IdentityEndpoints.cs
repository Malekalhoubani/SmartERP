using SmartERP.Identity.API.Filters;
using SmartERP.Identity.Services.Features.Authentication.Register;
using SmartERP.Identity.Services.Features.Authentication.Register.DTO;
using SmartERP.ServiceDefaults.FoundationOptions;
using SmartERP.Identity.Services.Features.Authentication.Login;
using SmartERP.Identity.Services.Features.Authentication.Login.DTO;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using SmartERP.Identity.DataAccess.Options;
using SmartERP.Identity.Services.Features.Authentication.Sessions;
using System.Security.Claims;
using SmartERP.Identity.API.Services;
using SmartERP.Identity.Services.Features.Authentication.ForgotPassword.DTO;
using SmartERP.Identity.Services.Interfaces.Authentication;
namespace SmartERP.Identity.API.Endpoints;

public static class IdentityEndpoints
{
    public static IEndpointRouteBuilder MapIdentityEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        var authenticationGroup = endpoints
            .MapGroup("/api/auth")
            .WithTags("Authentication");

        authenticationGroup
            .MapPost("/register", RegisterAsync)
            .AddEndpointFilter<ValidationFilter<RegisterRequestDto>>()
            .RequireRateLimiting(
                ServiceDefaultsExtensions.AuthenticationRateLimitPolicy);

        authenticationGroup
            .MapGet("/confirm-email", ConfirmEmailAsync)
            .RequireRateLimiting(
                ServiceDefaultsExtensions.AuthenticationRateLimitPolicy);

        authenticationGroup
            .MapPost("/login", LoginAsync)
            .AddEndpointFilter<ValidationFilter<LoginRequestDto>>()
            .RequireRateLimiting(
                ServiceDefaultsExtensions.AuthenticationRateLimitPolicy);

        authenticationGroup
            .MapPost("/forgot-password", ForgotPasswordAsync)
            .AddEndpointFilter<ValidationFilter<ForgotPasswordRequestDto>>()
            .RequireRateLimiting(
                ServiceDefaultsExtensions.AuthenticationRateLimitPolicy)
            .AllowAnonymous();

        authenticationGroup
            .MapPost("/refresh", RefreshAsync)
            .RequireRateLimiting(
                ServiceDefaultsExtensions.AuthenticationRateLimitPolicy);

        authenticationGroup
            .MapPost("/logout", LogoutAsync)
            .RequireRateLimiting(
                ServiceDefaultsExtensions.AuthenticationRateLimitPolicy);

        authenticationGroup
            .MapPost("/logout-all", LogoutAllAsync)
            .RequireAuthorization();

        authenticationGroup
            .MapGet("/sessions", GetSessionsAsync)
            .RequireAuthorization();

        authenticationGroup
            .MapDelete("/sessions/{sessionId:long}", RevokeSessionAsync)
            .RequireAuthorization();

        return endpoints;
    }

    private static async Task<IResult> RegisterAsync(
        RegisterRequestDto requestDto,
        IRegisterService registerService,
        HttpRequest httpRequest,
        CancellationToken cancellationToken)
    {
        var confirmationBaseUrl =
            $"{httpRequest.Scheme}://{httpRequest.Host}/api/auth/confirm-email";

        var response = await registerService.RegisterAsync(
            requestDto,
            confirmationBaseUrl,
            cancellationToken);

        return Results.Json(
            response,
            statusCode: StatusCodes.Status201Created);
    }

    private static async Task<IResult> ConfirmEmailAsync(
        int userId,
        string token,
        IRegisterService registerService,
        CancellationToken cancellationToken)
    {
        await registerService.ConfirmEmailAsync(
            userId,
            token,
            cancellationToken);

        return Results.Ok(
            new ConfirmEmailResponseDto(
                "Email confirmed successfully."));
    }

    private static async Task<IResult> LoginAsync(
        LoginRequestDto requestDto,
        ILoginService loginService,
        IOptions<RefreshTokenOptions> refreshTokenOptions,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await loginService.LoginAsync(
            requestDto,
            GetIpAddress(httpContext),
            httpContext.Request.Headers.UserAgent.ToString(),
            cancellationToken);

        WriteRefreshTokenCookie(
            httpContext.Response,
            refreshTokenOptions.Value,
            result.RefreshToken,
            result.RefreshTokenExpiresAtUtc);

        return Results.Ok(result.Response);
    }

    private static async Task<IResult> ForgotPasswordAsync(
        ForgotPasswordRequestDto requestDto,
        IForgotPasswordService forgotPasswordService,
        FrontendOriginResolver frontendOriginResolver,
        HttpRequest request,
        CancellationToken cancellationToken)
    {
        var frontendOrigin = frontendOriginResolver.Resolve(request);

        if (frontendOrigin is null)
        {
            return Results.BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Invalid frontend origin",
                Detail = "The request origin is missing or is not allowed."
            });
        }

        var response = await forgotPasswordService.ForgotPasswordAsync(
            requestDto,
            frontendOrigin,
            cancellationToken);

        return Results.Ok(response);
    }

    private static async Task<IResult> RefreshAsync(
        IAuthenticationSessionService sessionService,
        IOptions<RefreshTokenOptions> refreshTokenOptions,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var refreshToken = GetRequiredRefreshToken(
            httpContext.Request,
            refreshTokenOptions.Value.CookieName);

        var result = await sessionService.RefreshAsync(
            refreshToken,
            GetIpAddress(httpContext),
            cancellationToken);

        WriteRefreshTokenCookie(
            httpContext.Response,
            refreshTokenOptions.Value,
            result.RefreshToken,
            result.RefreshTokenExpiresAtUtc);

        return Results.Ok(result.Response);
    }

    private static async Task<IResult> LogoutAsync(
        IAuthenticationSessionService sessionService,
        IOptions<RefreshTokenOptions> refreshTokenOptions,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (httpContext.Request.Cookies.TryGetValue(
                refreshTokenOptions.Value.CookieName,
                out var refreshToken) &&
            !string.IsNullOrWhiteSpace(refreshToken))
        {
            await sessionService.RevokeByRefreshTokenAsync(
                refreshToken,
                GetIpAddress(httpContext),
                cancellationToken);
        }

        DeleteRefreshTokenCookie(
            httpContext.Response,
            refreshTokenOptions.Value.CookieName);

        return Results.NoContent();
    }

    private static async Task<IResult> LogoutAllAsync(
        ClaimsPrincipal user,
        IAuthenticationSessionService sessionService,
        IOptions<RefreshTokenOptions> refreshTokenOptions,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        await sessionService.RevokeAllSessionsAsync(
            GetRequiredIntClaim(user, "sub"),
            GetIpAddress(httpContext),
            cancellationToken);

        DeleteRefreshTokenCookie(
            httpContext.Response,
            refreshTokenOptions.Value.CookieName);

        return Results.NoContent();
    }

    private static async Task<IResult> GetSessionsAsync(
        ClaimsPrincipal user,
        IAuthenticationSessionService sessionService,
        CancellationToken cancellationToken)
    {
        var sessions = await sessionService.GetSessionsAsync(
            GetRequiredIntClaim(user, "sub"),
            GetRequiredLongClaim(user, "sid"),
            cancellationToken);

        return Results.Ok(sessions);
    }

    private static async Task<IResult> RevokeSessionAsync(
        long sessionId,
        ClaimsPrincipal user,
        IAuthenticationSessionService sessionService,
        IOptions<RefreshTokenOptions> refreshTokenOptions,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = GetRequiredIntClaim(user, "sub");
        var currentSessionId = GetRequiredLongClaim(user, "sid");

        await sessionService.RevokeSessionAsync(
            userId,
            sessionId,
            "Session revoked by user.",
            GetIpAddress(httpContext),
            cancellationToken);

        if (sessionId == currentSessionId)
        {
            DeleteRefreshTokenCookie(
                httpContext.Response,
                refreshTokenOptions.Value.CookieName);
        }

        return Results.NoContent();
    }

    private static string GetRequiredRefreshToken(
        HttpRequest request,
        string cookieName)
    {
        if (!request.Cookies.TryGetValue(cookieName, out var refreshToken) ||
            string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new UnauthorizedAccessException(
                "Refresh token cookie is required.");
        }

        return refreshToken;
    }

    private static int GetRequiredIntClaim(
        ClaimsPrincipal user,
        string claimType)
    {
        var value = user.FindFirstValue(claimType);

        return int.TryParse(value, out var parsedValue)
            ? parsedValue
            : throw new UnauthorizedAccessException(
                $"Required claim '{claimType}' is missing.");
    }

    private static long GetRequiredLongClaim(
        ClaimsPrincipal user,
        string claimType)
    {
        var value = user.FindFirstValue(claimType);

        return long.TryParse(value, out var parsedValue)
            ? parsedValue
            : throw new UnauthorizedAccessException(
                $"Required claim '{claimType}' is missing.");
    }

    private static string? GetIpAddress(HttpContext httpContext)
    {
        return httpContext.Connection.RemoteIpAddress?.ToString();
    }

    private static void WriteRefreshTokenCookie(
        HttpResponse response,
        RefreshTokenOptions options,
        string refreshToken,
        DateTimeOffset expiresAtUtc)
    {
        response.Cookies.Append(
            options.CookieName,
            refreshToken,
            CreateRefreshTokenCookieOptions(expiresAtUtc));
    }

    private static void DeleteRefreshTokenCookie(
        HttpResponse response,
        string cookieName)
    {
        response.Cookies.Delete(
            cookieName,
            CreateRefreshTokenCookieOptions(DateTimeOffset.UnixEpoch));
    }

    private static CookieOptions CreateRefreshTokenCookieOptions(
        DateTimeOffset expiresAtUtc)
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            IsEssential = true,
            Path = "/api/auth",
            Expires = expiresAtUtc
        };
    }
}
