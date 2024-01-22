using Sparcpoint.Abstract;
using Sparcpoint.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sparcpoint.Implementations
{
    public class ProductService : IProductService
    {
        public Task AddProductAsync(Product product)
        {
            throw new System.NotImplementedException();
        }

        public Task<Product> GetProductAsync(int productId)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<Product>> GetProductsAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
