using MoExpenseTracker.Core;

using CategoryModel = MoExpenseTracker.Models.Category;

namespace MoExpenseTracker.Features.V0.Category;

static class RequestHandlers
{
    public static async Task<IResult> CreateCategory(
        CategotyDataAccess dao,
       ICurrentUser currentUser,
        CreateCategoryDto dto)
    {

        var userId = currentUser.UserId();

        var category = await dao.ReadCategoryByName(userId, dto.Name);
        if (category is not null)
        {
            return Results.BadRequest<FailureResponse>(new("Category name already exists"));
        }

        var newCategory = new CategoryModel()
        {
            UserId = userId,
            Name = dto.Name,
            Description = dto.Description,
        };

        var response = await dao.CreateCategory(newCategory);
        if (response is null)
        {
            return Results.BadRequest<FailureResponse>(new("Coulkd not create category"));
        }

        return Results.Ok<SuccessResponseWithData<CategoryModel>>(new(response));
    }

    public static async Task<IResult> ListCategories(
        CategotyDataAccess dao,
        ICurrentUser currentUser,
        int pageNumber = 1,
        int pageSize = 10,
        string nameSearch = "",
        string sortBy = "id",
        string sortIn = "asc"
        )
    {
        var userId = currentUser.UserId();

        var pagination = new Pagination(pageNumber, pageSize);

        var categories = await dao.ListCategoriesByUserId(userId, pagination, nameSearch, sortBy, sortIn);
        var count = await dao.CountCategoriesByUserId(userId, nameSearch);

        return Results.Ok<PaginationResponse<List<CategoryModel>>>(new(categories, count, pagination));
    }

    public static async Task<IResult> ReadCategory(
        CategotyDataAccess dao,
        ICurrentUser currentUser,
        int categoryId)
    {
        var userId = currentUser.UserId();

        var category = await dao.ReadCategoryById(userId, categoryId);
        if (category is null)
        {
            return Results.Ok<FailureResponse>(new("Category not found"));
        }

        return Results.Ok<SuccessResponseWithData<CategoryModel>>(new(category));
    }

    public static async Task<IResult> UpdateCategory(
        CategotyDataAccess dao,
        ICurrentUser currentUser,
        int categoryId,
        UpdateCategoryDto dto)
    {
        var userId = currentUser.UserId();

        var category = await dao.ReadCategoryById(userId, categoryId);
        if (category is null)
        {
            return Results.Ok<FailureResponse>(new("Category not found"));
        }

        category.Name = dto.Name ?? category.Name;
        category.Description = dto.Description ?? category.Description;
        category.UpdatedAt = DateTime.UtcNow;

        var updatedCategory = await dao.UpdateCategory(category);
        if (updatedCategory is null)
        {
            return Results.Ok<FailureResponse>(new("Could update category, please try again"));
        }

        return Results.Ok<SuccessResponseWithData<CategoryModel>>(new(updatedCategory));
    }
}