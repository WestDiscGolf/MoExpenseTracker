using MoExpenseTracker.Core;
using MoExpenseTracker.Models;

namespace MoExpenseTracker.Features.V0.Auth;

class AuthController(AuthDao dao, AuthUtil util, IConfiguration configuration)
{
    // inject the config for accessing the jwt fileds
    public async Task<IResult> Signup(/* we could pass the validator here and use it at line 14 */SignupDto dto)
    {
        try
        {
            // manually doing the validation in the controller
            // we will do the login validation using the endpoint filter
            AuthSignValidation validator = new();
            var validation = validator.Validate(dto);
            Console.WriteLine("Hello there");


            if (!validation.IsValid)
            {
                System.Console.WriteLine(validation.Errors[0]);
                // to take the first one
                return Results.BadRequest<FailureResponse>(new(validation.Errors[0].ToString()));
                // return Results.ValidationProblem(validation.ToDictionary());

            }

            // we could inject the the validation service and pass an instance to the controller 
            // so that we don't have to create an instance
            // the best aproach will be to just inject it
            // validation ends here

            // check that there is no user with this email
            var isEmailTaken = await dao.IsExitingUser(dto.Email);
            if (isEmailTaken)
            {
                return Results.BadRequest<FailureResponse>(new("Email already taken"));
            }

            // hash user password
            var passwordHash = util.Hash(dto.Password);
            dto.Password = passwordHash;

            // insert record 
            var row = await dao.CreateUser(dto); // the number of rows inserted
            if (row < 1)
            {
                return Results.BadRequest<FailureResponse>(new("Could not create user"));
            }

            var user = await dao.GetUserByEmail(dto.Email);

            var authResponse = GetAuthResponse(user!);

            return Results.Ok<SuccessResponseWithData<AuthResponseDto>>(new(authResponse));
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Data);
            throw;
        }
    }

    public async Task<IResult> Login(LoginDto dto)
    {
        // find user with this email
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

        var authResponse = GetAuthResponse(user);

        return Results.Ok<SuccessResponseWithData<AuthResponseDto>>(new(authResponse));
    }

    private AuthResponseDto GetAuthResponse(User user)
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