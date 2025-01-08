namespace MoExpenseTracker.Features.V0.Account;

static class AccountEndpoint
{
    public static void AddAccountEndpoint(this IEndpointRouteBuilder app)
    {
        var userRoute = app.MapGroup("/accounts");

        userRoute.MapGet("/", RequestHandlers.ReadProfile).RequireAuthorization();

        userRoute.MapPut("/", RequestHandlers.UpdateProfile).RequireAuthorization();
    }
}