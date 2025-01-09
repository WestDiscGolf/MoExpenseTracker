using MoExpenseTracker.Core;

namespace MoExpenseTracker.Features.V0.Category;

static class Endpoint
{
    public static void AddCategoryEndpoint(this IEndpointRouteBuilder app)
    {
        var categoryRouth = app.MapGroup("/categories");

        // we can choose to inject IValidator<CreateCategoryDto> here as part of the parameters
        categoryRouth.MapPost("/", RequestHandlers.CreateCategory)
            .RequireAuthorization()
            .AddEndpointFilter<ValidationEndpointFilter<CreateCategoryDto>>();

        categoryRouth.MapGet("/", RequestHandlers.ListCategories)
            .RequireAuthorization();

        categoryRouth.MapGet("/{id:int}", RequestHandlers.ReadCategory)
            .RequireAuthorization();

        categoryRouth.MapPut("/{id:int}", RequestHandlers.UpdateCategory)
            .RequireAuthorization()
            .AddEndpointFilter<ValidationEndpointFilter<UpdateCategoryDto>>();
    }
}