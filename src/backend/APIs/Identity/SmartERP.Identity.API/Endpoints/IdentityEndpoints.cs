using SmartERP.Identity.API.Filters;
using SmartERP.Identity.Services.Features.Authentication.Register;
using SmartERP.Identity.Services.Features.Authentication.Register.DTO;
using SmartERP.ServiceDefaults.FoundationOptions;
using SmartERP.Identity.Services.Features.Authentication.Login;
using SmartERP.Identity.Services.Features.Authentication.Login.DTO;
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
        CancellationToken cancellationToken)
    {
        var response = await loginService.LoginAsync(
            requestDto,
            cancellationToken);

        return Results.Ok(response);
    }
}
