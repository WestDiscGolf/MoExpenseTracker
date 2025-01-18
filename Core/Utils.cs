namespace MoExpenseTracker.Core;

abstract class AppResponse(bool success)
{
    public bool Success { get; set; } = success;
}

class FailureResponse(string message) : AppResponse(false)
{
    public string Message { get; set; } = message;
}

class SuccessResponse(string message) : AppResponse(true)
{
    public string Message { get; set; } = message;
}

class SuccessResponseWithData<T>(T data) : AppResponse(true)
{
    public T Data { get; set; } = data;
}

class PaginationResponse<T>(T data, int count, Pagination pagination) : SuccessResponseWithData<T>(data)
{
    public int CurrentPageSize { get; } = pagination.PageSize;
    public int CurrentPageNumber { get; } = pagination.PageNumber;
    public int Count { get; } = count;
    public bool HasNextPage { get; } = pagination.PageNumber * pagination.PageSize < count;
}

class AppConfig
{
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public required string Key { get; set; }
    public required int Expires { get; set; }
}