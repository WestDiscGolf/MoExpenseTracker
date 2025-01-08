using Microsoft.EntityFrameworkCore;
using MoExpenseTracker.Data;
using CategoryModel = MoExpenseTracker.Models.Category;

namespace MoExpenseTracker.Features.Category;

class CategoryDao(DatabaseContext context)
{
    public async Task<CategoryModel?> ReadCategoryByName(int userId, string cateegoryName)
    {
        return await context.Categories.FirstOrDefaultAsync(
            (row) => row.UserId == userId
            && row.Name.ToLower().Equals(cateegoryName.ToLower()));
    }

    public async Task<CategoryModel> CreateCategory(CategoryModel category)
    {
        await context.Categories.AddAsync(category);
        await context.SaveChangesAsync();

        return (await ReadCategoryByName(category.UserId, category.Name))!;
    }

    public async Task<List<CategoryModel>> ListCategoriesByUserId(int userId)
    {
        return await context.Categories.Where(row => row.UserId == userId).ToListAsync();
    }

    public async Task<CategoryModel?> ReadCategoryById(int userId, int categoryId)
    {
        return await context.Categories.FirstOrDefaultAsync(
            (row) => row.UserId == userId
            && row.Id == categoryId);
    }

    public async Task<CategoryModel?> UpdateCategory(CategoryModel category)
    {
        context.Entry(category).State = EntityState.Modified;
        await context.SaveChangesAsync();

        return await ReadCategoryById(category.UserId, category.Id);
    }
}