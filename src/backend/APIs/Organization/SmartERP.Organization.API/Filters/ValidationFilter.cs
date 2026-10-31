using FluentValidation;

namespace SmartERP.Organization.API.Filters;

public sealed class ValidationFilter<TRequest> : IEndpointFilter
    where TRequest : class
{
    private readonly IValidator<TRequest> _validator;

    public ValidationFilter(IValidator<TRequest> validator)
    {
        _validator = validator;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context,EndpointFilterDelegate next)
    {
        var request = context.Arguments
            .OfType<TRequest>()
            .FirstOrDefault();

        if (request is null)
        {
            return await next(context);
        }

        var validationResult =
            await _validator.ValidateAsync(
                request,
                context.HttpContext.RequestAborted);

        if (validationResult.IsValid)
        {
            return await next(context);
        }

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
}