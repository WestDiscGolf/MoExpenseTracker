using System.Text;

using FluentValidation;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using MoExpenseTracker.Core;
using MoExpenseTracker.Data;
using MoExpenseTracker.Features;
using MoExpenseTracker.Features.V0.Account;
using MoExpenseTracker.Features.V0.Auth;
using MoExpenseTracker.Features.V0.Category;
using MoExpenseTracker.Features.V0.Expense;

namespace MoExpenseTracker;

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

        app.UseExceptionHandler(_ => { });
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        // add endpoints
        // app.AddFeatureEndpoints();
        app.AddVersionEndpoints();

        app.Run();
    }

}