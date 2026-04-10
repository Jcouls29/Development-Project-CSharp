using Sparcpoint.Inventory.Models;

namespace Sparcpoint.Inventory.Services
{
    /// <summary>
    /// EVAL: Service interface for category business logic.
    /// Supports creating categories, retrieving by ID, listing all, and navigating hierarchies.
    /// </summary>
    public interface ICategoryService
    {
        Task<Category> CreateCategoryAsync(Category category);
        Task<Category?> GetCategoryByIdAsync(int instanceId);
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<IEnumerable<Category>> GetChildCategoriesAsync(int parentCategoryId);
    }
}
