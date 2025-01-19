using System.Globalization;

using MoExpenseTracker.Core;
using MoExpenseTracker.Features.V0.Category;

using ExpenseModel = MoExpenseTracker.Models.Expense;

namespace MoExpenseTracker.Features.V0.Expense;

static class RequestHandlers
{
    public static async Task<IResult> CreateExpense(
        ExpenseDao expenseDao,
        CategoryDao categoryDao,
        HttpContext httpContext,
        CreateExpenseDto dto)
    {
        if (!(httpContext.User.Identity?.IsAuthenticated ?? false))
        {
            // Return Unauthorized if user is not authenticated
            return Results.Unauthorized();
        }

        // Find the claims for id and email
        var userId = int.Parse(httpContext.User.FindFirst("id")?.Value!);
        // email has been removed from the User claims

        var category = await categoryDao.ReadCategoryById(userId, dto.CategoryId);
        if (category is null)
        {
            return Results.Ok<FailureResponse>(new("Category not found"));
        }

        if (dto.Amount < 0)
        {
            return Results.Ok<FailureResponse>(new("Expense amount can not be in the negative"));
        }

        DateOnly parsedExpenseDate = DateOnly.FromDateTime(DateTime.Now);

        if (!string.IsNullOrEmpty(dto.ExpenseDate) && !DateOnly.TryParseExact(
                dto.ExpenseDate,
                "dd/MM/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out parsedExpenseDate))
        {
            return Results.Ok<FailureResponse>(new("Expense date must be of the format: dd/MM/yyyy"));
        }

        var newExpense = new ExpenseModel()
        {
            UserId = userId,
            Amount = dto.Amount,
            CategoryId = dto.CategoryId,
            Description = dto.Description?.ToLower() ?? "",
            ExpenseDate = parsedExpenseDate
        };

        var expense = await expenseDao.CreateExpense(newExpense);

        return Results.Ok<SuccessResponseWithData<ExpenseModel>>(new(expense));
    }

    public static async Task<IResult> ListExpenses(
        ExpenseDao expenseDao,
        HttpContext httpContext,
        int pageNumber = 1,
        int pageSize = 10,
        string nameSearch = "",
        string sortBy = "id",
        string sortIn = "asc")
    {
        if (!(httpContext.User.Identity?.IsAuthenticated ?? false))
        {
            // Return Unauthorized if user is not authenticated
            return Results.Unauthorized();
        }

        // Find the claims for id and email
        var userId = int.Parse(httpContext.User.FindFirst("id")?.Value!);

        var pagination = new Pagination(pageNumber, pageSize);

        var expenses = await expenseDao.ListExpensesByUserId(userId, pagination, nameSearch, sortBy, sortIn);

        var count = await expenseDao.CountExpensesByUserId(userId);

        return Results.Ok<PaginationResponse<List<ExpenseModel>>>(new(expenses, count, pagination));
    }

    public static async Task<IResult> ReadExpense(
        ExpenseDao expenseDao,
        HttpContext httpContext,
        int id)
    {
        if (!(httpContext.User.Identity?.IsAuthenticated ?? false))
        {
            // Return Unauthorized if user is not authenticated
            return Results.Unauthorized();
        }

        // Find the claims for id and email
        var userId = int.Parse(httpContext.User.FindFirst("id")?.Value!);
        // email has been removed from the User claims

        var expense = await expenseDao.ReadExpenseById(userId, id);
        if (expense is null)
        {
            return Results.Ok<FailureResponse>(new("Expense not found"));
        }

        return Results.Ok<SuccessResponseWithData<ExpenseModel>>(new(expense));
    }

    public static async Task<IResult> UpdateExpense(
        ExpenseDao expenseDao,
        CategoryDao categoryDao,
        HttpContext httpContext,
        int id,
        UpdateExpenseDto dto)
    {
        if (!(httpContext.User.Identity?.IsAuthenticated ?? false))
        {
            // Return Unauthorized if user is not authenticated
            return Results.Unauthorized();
        }

        // Find the claims for id and email
        var userId = int.Parse(httpContext.User.FindFirst("id")?.Value!);
        // email has been removed from the User claims

        var expense = await expenseDao.ReadExpenseById(userId, id);
        if (expense is null)
        {
            return Results.Ok<FailureResponse>(new("Expense not found"));
        }

        // assuming category id was passed, it must belong to the user
        if (dto.CategoryId is not null)
        {
            var category = await categoryDao.ReadCategoryById(userId, dto.CategoryId.Value);
            if (category is null)
            {
                return Results.Ok<FailureResponse>(new("Category not found"));
            }
        }

        // assuming user wants to update the expense date
        if (!string.IsNullOrEmpty(dto.ExpenseDate))
        {
            if (!DateOnly.TryParseExact(
                    dto.ExpenseDate,
                    "dd/MM/yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var parsedExpenseDate))
            {
                return Results.Ok<FailureResponse>(new("Expense date must be of the format: dd/MM/yyyy"));
            }

            expense.ExpenseDate = parsedExpenseDate;
        }


        if (dto.Amount is not null && dto.Amount.Value < 0)
        {
            return Results.Ok<FailureResponse>(new("Expense amount can not be in the negative"));
        }

        expense.CategoryId = dto.CategoryId ?? expense.CategoryId;
        expense.Amount = dto.Amount ?? expense.Amount;
        expense.Description = dto.Description?.ToLower() ?? expense.Description;
        expense.UpdatedAt = DateTime.UtcNow;

        var updatedExpense = await expenseDao.UpdateExpense(expense);

        return Results.Ok<SuccessResponseWithData<ExpenseModel>>(new(updatedExpense));
    }
}