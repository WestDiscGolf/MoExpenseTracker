using Microsoft.EntityFrameworkCore;

using MoExpenseTracker.Core;
using MoExpenseTracker.Data;

using ExpenseModel = MoExpenseTracker.Models.Expense;

namespace MoExpenseTracker.Features.V0.Expense;

class ExpenseDao(DatabaseContext context)
{
    public async Task<ExpenseModel?> ReadExpenseById(int userId, int expenseId)
    {
        return await context.Expenses
                    .FirstOrDefaultAsync(row => row.UserId == userId && row.Id == expenseId);
    }

    public async Task<ExpenseModel> CreateExpense(ExpenseModel expense)
    {
        context.Expenses.Add(expense);
        await context.SaveChangesAsync();

        return expense;
    }

    public async Task<List<ExpenseModel>> ListExpensesByUserId(
        int userId,
        Pagination pagination,
        string nameSearch = "",
        string sortBy = "id",
        string sortIn = "asc")
    {
        var records = context.Expenses.Where(row => row.UserId == userId);

        if (!string.IsNullOrEmpty(nameSearch))
        {
            records = records.Where(row => !string.IsNullOrEmpty(row.Description)
                                        && row.Description.ToLower().Contains(nameSearch));
        }

        if (sortBy.Equals("expense_date") && sortIn.Equals("desc"))
        {
            records = records.OrderByDescending(row => row.ExpenseDate);
        }
        else if (sortBy.Equals("expense_date") && sortIn.Equals("asc"))
        {
            records = records.OrderBy(row => row.ExpenseDate);
        }
        else if (sortBy.Equals("amount") && sortIn.Equals("desc"))
        {
            records = records.OrderByDescending(row => row.ExpenseDate);
        }
        else if (sortBy.Equals("amount") && sortIn.Equals("asc"))
        {
            records = records.OrderBy(row => row.ExpenseDate);
        }
        else if (sortBy.Equals("category") && sortIn.Equals("desc"))
        {
            records = records.OrderByDescending(row => row.CategoryId);
        }
        else if (sortBy.Equals("category") && sortIn.Equals("asc"))
        {
            records = records.OrderBy(row => row.CategoryId);
        }
        else if (sortBy.Equals("id") && sortIn.Equals("desc"))
        {
            records = records.OrderByDescending(row => row.Id);
        }
        else
        {
            records = records.OrderBy(row => row.Id);
        }

        return await records
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();
    }

    public async Task<int> CountExpensesByUserId(int userId, string nameSearch = "")
    {
        if (!string.IsNullOrEmpty(nameSearch))
        {
            return await context.Expenses
                                .CountAsync(row => row.UserId == userId
                                                && !string.IsNullOrEmpty(row.Description)
                                                && row.Description.ToLower().Contains(nameSearch));
        }

        return await context.Expenses.CountAsync(row => row.UserId == userId);
    }

    internal async Task<ExpenseModel> UpdateExpense(ExpenseModel expense)
    {

        context.Entry(expense).State = EntityState.Modified;
        await context.SaveChangesAsync();

        return expense;
    }
}