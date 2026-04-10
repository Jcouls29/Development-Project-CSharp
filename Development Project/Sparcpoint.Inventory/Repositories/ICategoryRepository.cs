using Sparcpoint.Inventory.Models;

namespace Sparcpoint.Inventory.Repositories
{
    /// <summary>
    /// EVAL: Repository interface for Category data access.
    /// Supports CRUD and hierarchical category relationships via [Instances].[CategoryCategories].
    /// </summary>
    public interface ICategoryRepository
    {
        Task<Category> CreateAsync(Category category);
        Task<Category?> GetByIdAsync(int instanceId);
        Task<IEnumerable<Category>> GetAllAsync();
        Task<IEnumerable<Category>> GetChildrenAsync(int parentCategoryId);
    }
}
