using Sparcpoint.Models.Requests;
using Sparcpoint.Models.Tables;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sparcpoint.Abstract
{
    public interface IProductService
    {
        Task<int> AddProductAsync(Product product);
        Task<IEnumerable<Product>> GetProductsAsync();
        Task<IEnumerable<Product>> GetProductsAsync(ProductRequest request);
        Task<Product> GetProductAsync(int productId);
    }
}