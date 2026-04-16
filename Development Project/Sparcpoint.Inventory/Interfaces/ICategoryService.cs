using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Interfaces
{
    public interface ICategoryService
    {
        Task<Models.CategoryDetailModel> CreateCategoryAsync(Models.CreateCategoryRequest request);
        Task<Models.CategoryDetailModel> GetCategoryAsync(int instanceId);
        Task<Models.PaginatedResult<Models.CategoryModel>> GetCategoriesAsync(int page, int pageSize);
        Task AddAttributeAsync(int instanceId, string key, string value);
        Task RemoveAttributeAsync(int instanceId, string key);
        Task AddParentCategoryAsync(int instanceId, int parentCategoryId);
        Task RemoveParentCategoryAsync(int instanceId, int parentCategoryId);
    }
}
