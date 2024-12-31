using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MoExpenseTracker.Core;
using MoExpenseTracker.Data;
using MoExpenseTracker.Features;
using MoExpenseTracker.Features.Account;
using MoExpenseTracker.Features.Auth;
using MoExpenseTracker.Features.Category;
using MoExpenseTracker.Features.Expense;


class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // gloabal exeption handler
        builder.Services.AddExceptionHandler<AppExceptionHandler>(); // for errors that we throw
        builder.Services.AddExceptionHandler<UnHandledExceptionHandler>(); // for errors that we could not handle

        // database connection
        var connectionString = builder.Configuration.GetConnectionString("MoExpenseTrackerDb");
        builder.Services.AddDbContext<DatabaseContext>(
            options => options.UseNpgsql(connectionString));

        // authentication and authorization
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer((options) =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = builder.Configuration.GetValue<string>("Jwt:Issuer"),
                ValidAudience = builder.Configuration.GetValue<string>("Jwt:Audience"),
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("Jwt:Key")!)),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
            };
            options.RequireHttpsMetadata = false;
        });

        builder.Services.AddAuthorization();

        // register depencies
        builder.Services.AddScoped<AuthController>();
        builder.Services.AddScoped<AuthDao>();
        builder.Services.AddScoped<AuthUtil>();

        builder.Services.AddScoped<AccountController>();
        builder.Services.AddScoped<AccountDao>();
        // this is use injectiing the account validation so that we can pass 
        // an instance into the controller
        // this is explicit
        builder.Services.AddScoped<IValidator<UpdateProfileDto>, AccountUpdateValidation>();
        

        builder.Services.AddScoped<CategoryController>();
        builder.Services.AddScoped<CategoryDao>();
        // we are going to inject the create category validation and use endpoint filter
        // builder.Services.AddValidatorsFromAssemblyContaining<CategoryCreationValidation>();
        // builder.Services.AddValidatorsFromAssemblyContaining(typeof(CategoryCreationValidation));
        // builder.Services.AddValidatorsFromAssemblyContaining<CategoryCreationValidation>(ServiceLifetime.Scoped);
        builder.Services.AddScoped<IValidator<CreateCategoryDto>, CategoryCreationValidation>();
        builder.Services.AddScoped<IValidator<UpdateCategoryDto>, CategoryUpdateValidation>();


        builder.Services.AddScoped<ExpenseController>();
        builder.Services.AddScoped<ExpenseDao>();
        builder.Services.AddScoped<IValidator<CreateExpenseDto>, ExpenseCreationValidation>();
        builder.Services.AddScoped<IValidator<UpdateExpenseDto>, ExpenseUpdateValidation>();


        var app = builder.Build();

        app.MapGet("/hello", () => "Hello");

        app.MapGet("/hello/{id:int}", (int id) => $"Hello - {id}")
            .AddEndpointFilter(async (invocationContext, next) =>
            {
                var id = invocationContext.GetArgument<int>(0);
                if (id < 0)
                {
                    return Results.Problem("Id can not be less than zero", statusCode: StatusCodes.Status400BadRequest);
                }

                return await next(invocationContext);
            });



        app.MapPost("/hello", (SomeDto dto) => dto)
            .AddEndpointFilter(async (invocationContext, next) =>
            {
                var dto = invocationContext.GetArgument<SomeDto>(0);
                if ((dto.Username ?? "").Length < 1 || (dto.Password ?? "").Length < 1)
                {
                    return Results.Problem(
                        "Username and paswsord required",
                        statusCode: StatusCodes.Status400BadRequest
                    );
                }

                if (dto.Username?.Length < 5)
                {
                    return Results.Problem(
                        "Username size must be above five characters",
                        statusCode: StatusCodes.Status400BadRequest
                    );

                }

                if (dto.Password?.Length < 5)
                {
                    return Results.Problem(
                        "Password size must be above five characters",
                        statusCode: StatusCodes.Status400BadRequest
                    );

                }
                System.Console.WriteLine("Hello action filter");
                return await next(invocationContext);
            });


        // a middleware that targets some route
        app.UseWhen(hctx => hctx.Request.Path.StartsWithSegments("/hello"), builder =>
        {
            builder.Use(async (context, next) =>
            {
                System.Console.WriteLine("Hello there, this is a middleware");
                await next();
            });
        });

        app.UseExceptionHandler(o => { });
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        // add endpoints
        app.AddFeatureEndpoints();

        app.Run();
    }

    // public record class SomeDto(string Username, string Password);
    public record SomeDto(string Username, string Password);

}
