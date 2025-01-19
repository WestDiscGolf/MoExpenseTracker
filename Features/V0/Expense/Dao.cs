using Microsoft.EntityFrameworkCore;

using MoExpenseTracker.Core;
using MoExpenseTracker.Data;

using ExpenseModel = MoExpenseTracker.Models.Expense;

namespace MoExpenseTracker.Features.V0.Expense;

class ExpenseDao(DatabaseContext context)
{
    public async Task<ExpenseModel?> ReadExpenseById(int userId, int expenseId)
    {
        return await context.Expenses.SingleOrDefaultAsync(row => row.UserId == userId && row.Id == expenseId);
    }

    public async Task<ExpenseModel> CreateExpense(ExpenseModel expense)
    {
        await context.Expenses.AddAsync(expense);
        await context.SaveChangesAsync();

        return (await ReadExpenseById(expense.UserId, expense.Id))!;
    }

    public async Task<List<ExpenseModel>> ListExpensesByUserId(int userId, Pagination pagination)
    {
        return await context.Expenses.Where(row => row.UserId == userId)
            .OrderBy(row => row.Id)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();
    }

    public async Task<int> CountExpensesByUserId(int userId)
    {
        return await context.Expenses.CountAsync(row => row.UserId == userId);
    }

    internal async Task<ExpenseModel> UpdateExpense(ExpenseModel expense)
    {

        context.Entry(expense).State = EntityState.Modified;
        await context.SaveChangesAsync();

        return (await ReadExpenseById(expense.UserId, expense.Id))!;
    }
}