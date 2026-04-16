using System.Collections.Generic;
using System.Threading.Tasks;
using Interview.DataEntities.Models;

namespace Interview.Services
{
    public interface ICategoryService
    {
        Task<int> CreateCategoryAsync(CategoryRequest request);
        Task AddProductToCategoryAsync(int productId, int categoryId);
        Task<IEnumerable<CategoryResponse>> GetCategoriesAsync();
        Task<IEnumerable<CategoryAttributeResponse>> GetCategoryAttributesAsync(int categoryId);
    }
}
