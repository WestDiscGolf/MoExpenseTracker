using MoExpenseTracker.Core;

namespace MoExpenseTracker.Features.V0.Expense;

static class ExpenseEndpoint
{
    public static void AddExpenseEndpoint(this IEndpointRouteBuilder app)
    {
        var expenseRoute = app.MapGroup("/expenses").AddEndpointFilter(async (context, next) =>
        {
            Console.WriteLine(context.HttpContext.Request.Path + " was called");
            return await next(context);
        });

        expenseRoute.MapPost("/", RequestHandlers.CreateExpense)
            .AddEndpointFilter<AppEndpointFilter<CreateExpenseDto>>()
            .RequireAuthorization();

        expenseRoute.MapGet("/", RequestHandlers.ListExpenses)
            .RequireAuthorization();

        expenseRoute.MapGet("/{id:int}", RequestHandlers.ReadExpense)
            .RequireAuthorization();

        expenseRoute.MapPut("/{id:int}", RequestHandlers.UpdateExpense)
            .AddEndpointFilter<AppEndpointFilter<UpdateExpenseDto>>()
            .RequireAuthorization();
    }
}