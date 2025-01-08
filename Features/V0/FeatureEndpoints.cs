using MoExpenseTracker.Features.V0.Account;
using MoExpenseTracker.Features.V0.Auth;
using MoExpenseTracker.Features.V0.Category;
using MoExpenseTracker.Features.V0.Expense;

namespace MoExpenseTracker.Features.V0;

static class FeatureEndpoints
{
    public static void AddFeatureEndpointsV0(this IEndpointRouteBuilder app)
    {
        // you can create a group router here and prefix it with the version
        app.AddAuthEndpoint();
        app.AddAccountEndpoint();
        app.AddCategoryEndpoint();
        app.AddExpenseEndpoint();
    }
}