using MoExpenseTracker.Core;
using MoExpenseTracker.Models;

namespace MoExpenseTracker.Features.V0.Auth;

static class RequestHandlers
{
    public static async Task<IResult> Signup(
        AuthDao dao,
        AuthUtil util,
        IConfiguration configuration,
        SignupDto dto
        )
    {
        // check that there is no user with this email
        dto.Email = dto.Email.ToLower();

        var isEmailTaken = await dao.IsExitingUser(dto.Email);
        if (isEmailTaken)
        {
            return Results.BadRequest<FailureResponse>(new("Email already taken"));
        }

        // hash user password
        var passwordHash = util.Hash(dto.Password);
        dto.Password = passwordHash;
        dto.Name = dto.Name.ToLower();

        // insert record 
        var row = await dao.CreateUser(dto); // the number of rows inserted
        if (row < 1)
        {
            return Results.BadRequest<FailureResponse>(new("Could not create user"));
        }

        var user = await dao.GetUserByEmail(dto.Email);

        var authResponse = GetAuthResponse(util, configuration, user!);

        return Results.Ok<SuccessResponseWithData<AuthResponseDto>>(new(authResponse));

    }

    public static async Task<IResult> Login(
        AuthDao dao,
        AuthUtil util,
        IConfiguration configuration,
        LoginDto dto)
    {
        // find user with this email
        dto.Email = dto.Email.ToLower();

        var user = await dao.GetUserByEmail(dto.Email);
        if (user == null)
        {
            return Results.BadRequest<FailureResponse>(
                new("Invalid credentials: email not found"));
        }

        // authenticate the password (compare password)
        var isAuthentic = util.Compare(dto.Password, user.Password);
        if (!isAuthentic)
        {
            return Results.BadRequest<FailureResponse>(
                new("Invalid credentials: incoreect password"));
        }

        // return success message and authorization token with partial user data
        var authResponse = GetAuthResponse(util, configuration, user);

        return Results.Ok<SuccessResponseWithData<AuthResponseDto>>(new(authResponse));
    }

    private static AuthResponseDto GetAuthResponse(
        AuthUtil util,
        IConfiguration configuration,
        User user)
    {
        var jwtConfig = new AppConfig
        {
            Issuer = configuration.GetValue<string>("Jwt:Issuer")!,
            Audience = configuration.GetValue<string>("Jwt:Audience")!,
            Key = configuration.GetValue<string>("Jwt:Key")!,
            Expires = configuration.GetValue<int>("Jwt:Expires")!,
        };

        var jwt = util.GenerateJwt(jwtConfig, user);

        var authResponse = new AuthResponseDto()
        {
            AccessToken = jwt,
            Email = user.Email,
            Id = user.Id,
            Name = user.Name,
        };

        return authResponse;
    }
}