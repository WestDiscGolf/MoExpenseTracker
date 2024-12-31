using Microsoft.EntityFrameworkCore;
using MoExpenseTracker.Models;

namespace MoExpenseTracker.Data;


/* 
class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
}
*/
class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

    public DbSet<User> Users { get; set; } = null!;

    public DbSet<Category> Categories { get; set; } = null!;

    public DbSet<Expense> Expenses { get; set; } = null!;
}