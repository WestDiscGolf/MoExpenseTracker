using MoExpenseTracker.Core;

namespace MoExpenseTracker.Features.V0.Auth;

static class AuthEndpoint
{
    public static void AddAuthEndpoint(this IEndpointRouteBuilder app)
    {
        var authRoute = app.MapGroup("/auth");

        authRoute.MapPost("/signup", RequestHandlers.Signup)
            .AddEndpointFilter<ValidationEndpointFilter<SignupDto>>();

        authRoute.MapPost("/login", RequestHandlers.Login)
            .AddEndpointFilter<ValidationEndpointFilter<LoginDto>>();
    }
}