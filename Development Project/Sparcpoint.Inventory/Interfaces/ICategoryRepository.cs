using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Interfaces
{
    public interface ICategoryRepository
    {
        Task<int> CreateAsync(Models.CreateCategoryRequest request);
        Task<Models.CategoryDetailModel> GetByIdAsync(int instanceId);
        Task<Models.PaginatedResult<Models.CategoryModel>> GetAllAsync(int page, int pageSize);
        Task AddAttributeAsync(int instanceId, string key, string value);
        Task RemoveAttributeAsync(int instanceId, string key);
        Task AddParentCategoryAsync(int instanceId, int parentCategoryId);
        Task RemoveParentCategoryAsync(int instanceId, int parentCategoryId);
    }
}
