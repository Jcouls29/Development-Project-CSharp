using System.Collections.Generic;
using System.Threading.Tasks;
using Interview.DataEntities.Models;

namespace Interview.Services
{
    public interface IProductService
    {
        Task<int> CreateProductAsync(ProductRequest request);
        Task<IEnumerable<ProductResponse>> SearchProductsAsync(ProductSearchRequest request);
    }
}
