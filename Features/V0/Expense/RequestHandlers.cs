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
        ICurrentUser currentUser,
        CreateExpenseDto dto)
    {
        var userId = currentUser.UserId();

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
        ICurrentUser currentUser,
        int pageNumber = 1,
        int pageSize = 10,
        string nameSearch = "",
        string sortBy = "id",
        string sortIn = "asc")
    {
        var userId = currentUser.UserId();

        var pagination = new Pagination(pageNumber, pageSize);

        var expenses = await expenseDao.ListExpensesByUserId(userId, pagination, nameSearch, sortBy, sortIn);

        var count = await expenseDao.CountExpensesByUserId(userId);

        return Results.Ok<PaginationResponse<List<ExpenseModel>>>(new(expenses, count, pagination));
    }

    public static async Task<IResult> ReadExpense(
        ExpenseDao expenseDao,
        ICurrentUser currentUser,
        int expenseId)
    {

        var userId = currentUser.UserId();

        var expense = await expenseDao.ReadExpenseById(userId, expenseId);
        if (expense is null)
        {
            return Results.Ok<FailureResponse>(new("Expense not found"));
        }

        return Results.Ok<SuccessResponseWithData<ExpenseModel>>(new(expense));
    }

    public static async Task<IResult> UpdateExpense(
        ExpenseDao expenseDao,
        CategoryDao categoryDao,
        ICurrentUser currentUser,
        int expenseId,
        UpdateExpenseDto dto)
    {

        var userId = currentUser.UserId();

        var expense = await expenseDao.ReadExpenseById(userId, expenseId);
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