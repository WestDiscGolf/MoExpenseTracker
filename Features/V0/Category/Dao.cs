using Microsoft.EntityFrameworkCore;

using MoExpenseTracker.Core;
using MoExpenseTracker.Data;

using CategoryModel = MoExpenseTracker.Models.Category;

namespace MoExpenseTracker.Features.V0.Category;

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

    public async Task<List<CategoryModel>> ListCategoriesByUserId(
        int userId,
        Pagination pagination,
        string nameSearch = "",
        string sortBy = "id",
        string sortIn = "asc")
    {
        nameSearch = nameSearch.ToLower();
        sortBy = sortBy.ToLower();
        sortIn = sortIn.ToLower();

        var records = context.Categories.Where(row => row.UserId == userId);

        if (!string.IsNullOrEmpty(nameSearch))
        {
            records = records.Where(row => row.Name.ToLower().Contains(nameSearch));
        }

        // sort can be done by id or name, and asc and desc
        if (sortBy.Equals("name") && sortIn.Equals("desc"))
        {
            records = records.OrderByDescending(row => row.Name);
        }
        else if (sortBy.Equals("name") && sortIn.Equals("asc"))
        {
            records = records.OrderBy(row => row.Name);
        }
        else if (sortBy.Equals("id") && sortIn.Equals("desc"))
        {
            records = records.OrderByDescending(row => row.Id);
        }
        else
        {
            records = records.OrderBy(row => row.Id);
        }

        return await records
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();
    }

    public async Task<int> CountCategoriesByUserId(int userId, string nameSearch = "")
    {
        if (!string.IsNullOrEmpty(nameSearch))
        {
            return await context.Categories.CountAsync((row) =>
                row.UserId == userId && row.Name.ToLower().Contains(nameSearch.ToLower()));
        }


        return await context.Categories.CountAsync(row => row.UserId == userId);
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