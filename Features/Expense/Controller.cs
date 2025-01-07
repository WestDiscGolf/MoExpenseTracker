using System.Globalization;
using MoExpenseTracker.Core;
using MoExpenseTracker.Features.Category;
using ExpenseModel = MoExpenseTracker.Models.Expense;

namespace MoExpenseTracker.Features.Expense;

class ExpenseController(ExpenseDao expenseDao, CategoryDao categoryDao)
{
    public async Task<IResult> CreateExpense(HttpContext httpContext, CreateExpenseDto dto)
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

        DateTime parsedExpenseDate = DateTime.UtcNow;

        if (!string.IsNullOrEmpty(dto.ExpenseDate))
        {
            if (!DateTime.TryParseExact(
                dto.ExpenseDate,
                "dd/MM/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out parsedExpenseDate))
            {
                return Results.Ok<FailureResponse>(new("Expense date must be of the format: dd/MM/yyyy"));
            }

            parsedExpenseDate = parsedExpenseDate.ToUniversalTime();
        }


        var newExpense = new ExpenseModel()
        {
            UserId = userId,
            Amount = dto.Amount,
            CategoryId = dto.CategoryId,
            Description = dto.Description ?? "",
            ExpenseDate = parsedExpenseDate
        };

        var expense = await expenseDao.CreateExpense(newExpense);

        return Results.Ok<SuccessResponseWithData<ExpenseModel>>(new(expense));
    }

    public async Task<IResult> ListExpenses(HttpContext httpContext)
    {
        if (!(httpContext.User.Identity?.IsAuthenticated ?? false))
        {
            // Return Unauthorized if user is not authenticated
            return Results.Unauthorized();
        }

        // Find the claims for id and email
        var userId = int.Parse(httpContext.User.FindFirst("id")?.Value!);
        // email has been removed from the User claims

        var expenses = await expenseDao.ListCategoriesByUserId(userId);

        return Results.Ok<SuccessResponseWithData<List<ExpenseModel>>>(new(expenses));
    }

    public async Task<IResult> ReadExpense(HttpContext httpContext, int id)
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

    public async Task<IResult> UpdateExpense(HttpContext httpContext, int id, UpdateExpenseDto dto)
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
        DateTime parsedExpenseDate = expense.ExpenseDate!.Value;
        if (!string.IsNullOrEmpty(dto.ExpenseDate))
        {
            if (!DateTime.TryParseExact(
                dto.ExpenseDate,
                "dd/MM/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out parsedExpenseDate))
            {
                return Results.Ok<FailureResponse>(new("Expense date must be of the format: dd/MM/yyyy"));
            }

            parsedExpenseDate = parsedExpenseDate.ToUniversalTime();
        }

        if (dto.Amount is not null && dto.Amount.Value < 0)
        {
            return Results.Ok<FailureResponse>(new("Expense amount can not be in the negative"));
        }

        expense.CategoryId = dto.CategoryId ?? expense.CategoryId;
        expense.Amount = dto.Amount ?? expense.Amount;
        expense.Description = dto.Description ?? expense.Description;
        expense.ExpenseDate = parsedExpenseDate;
        expense.UpdatedAt = DateTime.UtcNow;

        var updatedExpense = await expenseDao.UpdateExpense(expense);

        return Results.Ok<SuccessResponseWithData<ExpenseModel>>(new(updatedExpense));
    }
}