using Sparcpoint.Core.Persistence.Entity.Sparcpoint.Entities;

namespace Sparcpoint.Infrastructure.Services.Interfaces
{
    public interface ICategoryService
    {
        public Task<List<Category>> GetAllCategoriesAsync();

        public Task<Category?> GetCategoryByIdAsync(int id);

        public Task<Category?> GetCategoryByNameAsync(string name);

        public Task<bool> CreateCategoryAsync(Category category);

        public Task<Category?> UpdateCategoryAsync(Category category);

        public Task<bool> DeleteCategoryAsync(int id);

        public Task<List<Category>> SearchCategoriesByNameAsync(string name);
    }
}