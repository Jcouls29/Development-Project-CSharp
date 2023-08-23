using Sparcpoint.Inventory.Domain.Entities.Instances;
using Sparcpoint.Inventory.Domain.Models.Instances;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Application.Services
{
    public interface IProductService
    {
        Task<Product> GetAsync(int id);
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product> AddProductAsync(Product item);
        Task<Product> AddProductAttributeAsync(int productId, string key, string value);
        Task<Product> AddProductAttributesAsync(int productId, params (string key, string value)[] attributes);
        Task<Product> AddProductCategoryAsync(int productId, int categoryId);
        Task<Product> AddProductCategoriesAsync(int productId, params int[] categoryId);
        Task<IEnumerable<Product>> SearchProductsAsync(ProductSearchModel model);
    }
}
