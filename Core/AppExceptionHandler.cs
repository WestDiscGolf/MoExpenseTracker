using Microsoft.AspNetCore.Diagnostics;

namespace MoExpenseTracker.Core;

class UnHandledExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        // this is to handle any bad request exception that was not throw by us
        if (exception is BadHttpRequestException e)
        {
            httpContext.Response.StatusCode = e.StatusCode;
            await httpContext.Response.WriteAsJsonAsync<FailureResponse>(
                new(e.Message),
                cancellationToken);

            return true;

        }

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync<FailureResponse>(
            new("Something went wrong"),
            cancellationToken);

        return true;
    }
}

class AppExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is AppException e)
        {
            httpContext.Response.StatusCode = e.StatusCode;
            await httpContext.Response.WriteAsJsonAsync<FailureResponse>(
                new(e.Message),
                cancellationToken);

            return true;
        }

        return false;
    }
}
