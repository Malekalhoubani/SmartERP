using FluentValidation;
using SmartERP.Identity.Services.Features.Authentication.Register;
using SmartERP.Identity.Services.Features.Authentication.Register.DTO;
using SmartERP.ServiceDefaults.FoundationOptions;

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
            .RequireRateLimiting(
                ServiceDefaultsExtensions.AuthenticationRateLimitPolicy);

        authenticationGroup
            .MapGet("/confirm-email", ConfirmEmailAsync)
            .RequireRateLimiting(
                ServiceDefaultsExtensions.AuthenticationRateLimitPolicy);

        return endpoints;
    }

    private static async Task<IResult> RegisterAsync(
        RegisterRequestDto requestDto,
        IValidator<RegisterRequestDto> validator,
        IRegisterService registerService,
        HttpRequest httpRequest,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(
            requestDto,
            cancellationToken);

        if (!validationResult.IsValid)
        {
            var validationErrors = validationResult.Errors
                .GroupBy(error => error.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group
                        .Select(error => error.ErrorMessage)
                        .Distinct()
                        .ToArray());

            return Results.ValidationProblem(validationErrors);
        }

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
}
