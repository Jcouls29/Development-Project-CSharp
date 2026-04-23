using Interview.Web.Models.Product;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interview.Web.Services
{
    public interface IProductService
    {
        //Create products
        Task<int> CreateProductAsync(CreateProductRequest request);

        // Search Products
        Task<IEnumerable<ProductDto>> SearchProductsAsync(ProductSearchRequest request);
    }
}
