using System.Collections.Generic;
using System.Threading.Tasks;
using Sparcpoint.Inventory.Models.Domain;

namespace Sparcpoint.Inventory.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<Category> CreateCategoryAsync(Category category);
        Task<Category?> GetCategoryByIdAsync(int instanceId);
        Task<List<Category>> GetAllCategoriesAsync();
        Task<List<Category>> GetCategoriesByIdsAsync(List<int> categoryIds);
    }
}