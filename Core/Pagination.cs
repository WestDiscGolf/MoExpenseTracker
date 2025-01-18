namespace MoExpenseTracker.Core;

class Pagination(int pageNumber = 1, int pageSize = 10)
{
    public int PageNumber { get; set; } = pageNumber < 1 ? 1 : pageNumber;
    public int PageSize { get; set; } = pageSize < 1 ? 10 : pageSize;
}
