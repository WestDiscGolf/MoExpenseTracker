
using FluentValidation;

namespace MoExpenseTracker.Core;

class AppEndpointFilter<T> : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();
        if (validator is null)
        {
            return Results.BadRequest<FailureResponse>(new("Could not find type to validate here"));
        }

        // from here we know that the dto is at position 2
        var dto = context.Arguments
            .OfType<T>()
            .FirstOrDefault(t => t?.GetType() == typeof(T));

        if (dto is null)
        {
            return Results.BadRequest<FailureResponse>(new("Request body required"));
        }

        var result = await validator.ValidateAsync(dto);
        if (!result.IsValid)
        {
            return Results.BadRequest<FailureResponse>(new(result.Errors[0].ToString()));
        }

        return await next(context);
    }
}