namespace MoExpenseTracker.Core;

public class AppException(string message, int statusCode = StatusCodes.Status400BadRequest) : Exception(message)
{
    public int StatusCode { get; set; } = statusCode;
}