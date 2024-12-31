using MoExpenseTracker.Features.Account;
using MoExpenseTracker.Features.Auth;
using MoExpenseTracker.Features.Category;
using MoExpenseTracker.Features.Expense;

namespace MoExpenseTracker.Features;

static class FeatureEndpoints
{
    public static void AddFeatureEndpoints(this IEndpointRouteBuilder app)
    {
        // you can create a group router here and prefix it with the version
        app.MapGet("/", () => Results.Ok("Mo Expense tracker pinging-ponging!!!"));
        app.AddAuthEndpoint();
        app.AddAccountEndpoint();
        app.AddCategoryEndpoint();
        app.AddExpenseEndpoint();
    }
}
