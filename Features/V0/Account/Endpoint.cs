
using FluentValidation;


namespace MoExpenseTracker.Features.V0.Account;

static class AccountEndpoint
{
    public static void AddAccountEndpoint(this IEndpointRouteBuilder app)
    {
        var userRoute = app.MapGroup("/accounts");

        userRoute.MapGet("/", async (AccountController controller, HttpContext context) =>
        {
            return await controller.ReadProfile(context);
        }).RequireAuthorization();

        userRoute.MapPut("/", async (
            IValidator<UpdateProfileDto> validator,
            AccountController controller,
            HttpContext context,
            UpdateProfileDto dto) =>
        {
            return await controller.UpdateProfile(validator, context, dto);
        }).RequireAuthorization();
    }
}