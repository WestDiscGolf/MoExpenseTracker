using FluentValidation;

using MoExpenseTracker.Core;
using MoExpenseTracker.Features.V0.Auth;

namespace MoExpenseTracker.Features.V0.Account;

class AccountController(AccountDao accountDao, AuthDao authDao)
{
    public async Task<IResult> ReadProfile(HttpContext httpContext)
    {
        // Check if the user is authenticated
        if (!(httpContext.User.Identity?.IsAuthenticated ?? false))
        {
            // Return Unauthorized if user is not authenticated
            return Results.Unauthorized();
        }

        // Find the claims for id and email
        var userId = int.Parse(httpContext.User.FindFirst("id")?.Value!);

        var user = await accountDao.ReadProfile(userId);
        if (user == null)
        {
            return Results.Unauthorized();
        }

        var profile = new UserProfileDto(user);

        // Return user profile data
        return Results.Ok<SuccessResponseWithData<UserProfileDto>>(new(profile));
    }

    public async Task<IResult> UpdateProfile(
        IValidator<UpdateProfileDto> validator,
        HttpContext httpContext,
        UpdateProfileDto dto)
    {
        // injected validator instance
        var result = await validator.ValidateAsync(dto);
        if (!result.IsValid)
        {
            return Results.Ok<FailureResponse>(new(result.Errors[0].ToString()));
        }

        // Check if the user is authenticated
        if (!(httpContext.User.Identity?.IsAuthenticated ?? false))
        {
            // Return Unauthorized if user is not authenticated
            return Results.Unauthorized();
        }

        // Find the claims for id and email
        var userId = int.Parse(httpContext.User.FindFirst("id")?.Value!);

        var user = await accountDao.ReadProfile(userId);
        if (user == null)
        {
            return Results.Unauthorized();
        }

        var emailUser = await authDao.GetUserByEmail(dto.Email);
        if (emailUser is not null && !emailUser.Email.Equals(user.Email))
        {
            return Results.Ok<FailureResponse>(new("Email already taken"));
        }

        user.Name = dto.Name;
        user.Email = dto.Email;
        user.UpdatedAt = DateTime.UtcNow;

        var updatedUser = await accountDao.UpdateProfile(user);
        var profile = new UserProfileDto(updatedUser!);

        // Return user profile data
        return Results.Ok<SuccessResponseWithData<UserProfileDto>>(new(profile));
    }
}