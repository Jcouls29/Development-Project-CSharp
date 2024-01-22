using Sparcpoint.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sparcpoint.Abstract
{
    public interface IProductService
    {
        Task AddProductAsync(Product product);
        Task<IEnumerable<Product>> GetProductsAsync();
        Task<Product> GetProductAsync(int productId);
    }
}
