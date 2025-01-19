using Microsoft.EntityFrameworkCore;

using MoExpenseTracker.Data;
using MoExpenseTracker.Models;

namespace MoExpenseTracker.Features.V0.Account;

class AccountDataAccess(DatabaseContext context)
{
    public async Task<User?> ReadProfile(int id)
    {
        return await context.Users.FirstOrDefaultAsync(row => row.Id == id);
    }

    public async Task<User?> UpdateProfile(User user)
    {
        context.Entry(user).State = EntityState.Modified;
        await context.SaveChangesAsync();

        return user;
    }
}