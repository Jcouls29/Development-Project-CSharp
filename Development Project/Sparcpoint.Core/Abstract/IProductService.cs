using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sparcpoint
{
    public interface IProductService
    {
        Task<Product> CreateProductAsync(Product product);
        Task<Product> GetProductByIdAsync(int instanceId);
        Task<IEnumerable<Product>> SearchProductsAsync(ProductSearchCriteria criteria);
    }
}
