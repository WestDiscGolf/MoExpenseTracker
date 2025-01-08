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

        expenseRoute.MapPost("/", async (ExpenseController controller, HttpContext httpContext, CreateExpenseDto dto) =>
        {
            return await controller.CreateExpense(httpContext, dto);
        })
        .AddEndpointFilter<AppEndpointFilter<CreateExpenseDto>>()
        .RequireAuthorization();

        expenseRoute.MapGet("/", async (ExpenseController controller, HttpContext httpContext) =>
        {
            return await controller.ListExpenses(httpContext);
        }).RequireAuthorization();

        expenseRoute.MapGet("/{id:int}", async (ExpenseController controller, HttpContext httpContext, int id) =>
        {
            return await controller.ReadExpense(httpContext, id);
        }).RequireAuthorization();

        expenseRoute.MapPut("/{id:int}", async (ExpenseController controller, HttpContext httpContext, int id, UpdateExpenseDto dto) =>
        {
            return await controller.UpdateExpense(httpContext, id, dto);
        })
        .AddEndpointFilter<AppEndpointFilter<UpdateExpenseDto>>()
        .RequireAuthorization();
    }
}