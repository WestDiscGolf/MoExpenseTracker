using Microsoft.EntityFrameworkCore;

using MoExpenseTracker.Data;
using MoExpenseTracker.Models;

namespace MoExpenseTracker.Features.V0.Auth;

class AuthDao(DatabaseContext context)
{
    public async Task<bool> IsExitingUser(string email)
    {
        var user = await context.Users.FirstOrDefaultAsync(row => row.Email == email);

        // TODO: Look into how to handle execeptions
        return user != null;
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        return await context.Users.FirstOrDefaultAsync(row => row.Email == email);
    }

    public async Task<int> CreateUser(SignupDto dto)
    {
        User user = new()
        {
            Email = dto.Email.ToLower(),
            Name = dto.Name,
            Password = dto.Password
        };

        context.Users.Add(user);
        return await context.SaveChangesAsync();
    }
}