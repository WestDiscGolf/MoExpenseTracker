
using FluentValidation;

namespace MoExpenseTracker.Features.Account;

static class AccountEndpoint
{
    public static void AddAccountEndpoint(this IEndpointRouteBuilder app)
    {
        var userRoute = app.MapGroup("/accounts");

        userRoute.MapGet("/", async (AccountController controller, HttpContext context) =>
        {
            return await controller.ReadProfile(context);
            // throw new Exception("Huhu");
            // throw new AppException("We threw some cool error", StatusCodes.Status305UseProxy);
            // throw new AppException("We threw some cool error");

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