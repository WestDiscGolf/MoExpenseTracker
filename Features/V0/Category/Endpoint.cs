using MoExpenseTracker.Core;

namespace MoExpenseTracker.Features.V0.Category;

static class Endpoint
{
    public static void AddCategoryEndpoint(this IEndpointRouteBuilder app)
    {
        var categoryRouth = app.MapGroup("/categories");

        // we can choose to inject IValidator<CreateCategoryDto> here as part of the parameters
        categoryRouth.MapPost("/", RequestHandlers.CreateCategory)
            .AddEndpointFilter<AppEndpointFilter<CreateCategoryDto>>()
            .RequireAuthorization();

        categoryRouth.MapGet("/", RequestHandlers.ListCategories)
            .RequireAuthorization();

        categoryRouth.MapGet("/{id:int}", RequestHandlers.ReadCategory)
            .RequireAuthorization();

        categoryRouth.MapPut("/{id:int}", RequestHandlers.UpdateCategory)
            .AddEndpointFilter<AppEndpointFilter<UpdateCategoryDto>>()
            .RequireAuthorization();
    }
}