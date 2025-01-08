using MoExpenseTracker.Core;

namespace MoExpenseTracker.Features.V0.Category;

static class Endpoint
{
    public static void AddCategoryEndpoint(this IEndpointRouteBuilder app)
    {
        var categoryRouth = app.MapGroup("/categories");

        // we can choose to inject IValidator<CreateCategoryDto> here as part of the parameters
        categoryRouth.MapPost("/", async (
            CategoryController controller,
            HttpContext httpContext,
            CreateCategoryDto dto) =>
        {
            return await controller.CreateCategory(httpContext, dto);
        })
        .AddEndpointFilter<AppEndpointFilter<CreateCategoryDto>>()
        /* .AddEndpointFilter(async (invocationContext, next) =>
        {
            // from here we know that the dto is at position 2
            // var dto = invocationContext.GetArgument<CreateCategoryDto>(2);
            if (invocationContext.Arguments[2] is not CreateCategoryDto dto)
            {
                return Results.BadRequest<FailureResponse>(new("Request body required"));
            }

            System.Console.WriteLine(new { dto.Name, dto.Description });

            // var validator = invocationContext.HttpContext.RequestServices.GetService<IValidator<CreateCategoryDto>>();
            // var validator = invocationContext.HttpContext.RequestServices.GetRequiredService<IValidator<CreateCategoryDto>>();
            var validator = invocationContext
               .HttpContext
               .RequestServices
               .GetService<IValidator<CreateCategoryDto>>();
            // .GetRequiredService<IValidator<CreateCategoryDto>>();

            if (validator is null)
            {
                return Results.BadRequest<FailureResponse>(new("Could not find type to validate"));
            }

            Console.WriteLine(dto);

            var result = await validator.ValidateAsync(dto);
            Console.WriteLine(result);
            if (!result.IsValid)
            {
                return Results.BadRequest<FailureResponse>(new(result.Errors[0].ToString()));
            }

            return await next(invocationContext);
        }) */
        .RequireAuthorization();

        categoryRouth.MapGet("/", async (CategoryController controller, HttpContext httpContext) =>
        {
            return await controller.ListCategories(httpContext);
        }).RequireAuthorization();

        categoryRouth.MapGet("/{id:int}", async (CategoryController controller, HttpContext httpContext, int id) =>
        {
            return await controller.ReadCategory(httpContext, id);
        }).RequireAuthorization();

        categoryRouth.MapPut("/{id:int}", async (CategoryController controller, HttpContext httpContext, int id, UpdateCategoryDto dto) =>
        {
            return await controller.UpdateCategory(httpContext, id, dto);
        })
            .AddEndpointFilter<AppEndpointFilter<UpdateCategoryDto>>()
            .RequireAuthorization();
    }
}