using FluentValidation;

namespace SmartERP.Identity.API.Filters;

public sealed class ValidationFilter<TRequest> : IEndpointFilter
    where TRequest : class
{
    private readonly IValidator<TRequest> _validator;

    public ValidationFilter(IValidator<TRequest> validator)
    {
        _validator = validator;
    }

    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        TRequest? request = null;

        foreach (var argument in context.Arguments)
        {
            if (argument is TRequest requestArgument)
            {
                request = requestArgument;
                break;
            }
        }

        if (request is null)
        {
            return await next(context);
        }

        var validationResult = await _validator.ValidateAsync(
            request,
            context.HttpContext.RequestAborted);

        if (validationResult.IsValid)
        {
            return await next(context);
        }

        var groupedErrors = new Dictionary<string, List<string>>();

        foreach (var failure in validationResult.Errors)
        {
            if (!groupedErrors.TryGetValue(
                    failure.PropertyName,
                    out var messages))
            {
                messages = [];
                groupedErrors[failure.PropertyName] = messages;
            }

            if (!messages.Contains(failure.ErrorMessage))
            {
                messages.Add(failure.ErrorMessage);
            }
        }

        var validationErrors = new Dictionary<string, string[]>();

        foreach (var error in groupedErrors)
        {
            validationErrors[error.Key] = error.Value.ToArray();
        }

        return Results.ValidationProblem(validationErrors);
    }
}
