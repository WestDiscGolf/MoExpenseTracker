using MoExpenseTracker.Core;

namespace MoExpenseTracker.Features.V0.Auth;

static class AuthEndpoint
{
    public static void AddAuthEndpoint(this IEndpointRouteBuilder app)
    {
        var authRoute = app.MapGroup("/auth");

        authRoute.MapPost("/signup", async (AuthController controller, SignupDto dto) =>
        {
            return await controller.Signup(dto);
        });

        authRoute.MapPost("/login", async (AuthController controller, LoginDto dto) =>
        {
            return await controller.Login(dto);
        }).AddEndpointFilter(async (invocationContext, next) =>
        {
            var dto = invocationContext.GetArgument<LoginDto>(1);
            if (dto is null)
            {
                return Results.BadRequest<FailureResponse>(new("Request body required"));
            }

            AuthLoginValidation validation = new();
            var result = await validation.ValidateAsync(dto);

            if (!result.IsValid)
            {
                return Results.BadRequest<FailureResponse>(new(result.Errors[0].ToString()));
            }

            return await next(invocationContext);
        });
    }
}