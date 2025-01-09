using FluentValidation;

using MoExpenseTracker.Core;
using MoExpenseTracker.Features.V0.Auth;

namespace MoExpenseTracker.Features.V0.Account;

static class RequestHandlers
{
    public static async Task<IResult> ReadProfile(
        AccountDao accountDao,
        HttpContext httpContext)
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

    public static async Task<IResult> UpdateProfile(
        AccountDao accountDao,
        AuthDao authDao,
        HttpContext httpContext,
        UpdateProfileDto dto)
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