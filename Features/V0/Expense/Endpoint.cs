using MoExpenseTracker.Core;

namespace MoExpenseTracker.Features.V0.Expense;

static class ExpenseEndpoint
{
    public static void AddExpenseEndpoint(this IEndpointRouteBuilder app)
    {
        var expenseRoute = app.MapGroup("/expenses");

        expenseRoute.MapPost("/", RequestHandlers.CreateExpense)
            .RequireAuthorization()
            .AddEndpointFilter<ValidationEndpointFilter<CreateExpenseDto>>();

        expenseRoute.MapGet("/", RequestHandlers.ListExpenses)
            .RequireAuthorization();

        expenseRoute.MapGet("/{id:int}", RequestHandlers.ReadExpense)
            .RequireAuthorization();

        expenseRoute.MapPut("/{id:int}", RequestHandlers.UpdateExpense)
            .RequireAuthorization()
            .AddEndpointFilter<ValidationEndpointFilter<UpdateExpenseDto>>();
    }
}