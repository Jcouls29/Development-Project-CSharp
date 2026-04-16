using Sparcpoint.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sparcpoint
{
    public interface IProductRepository
    {
        Task<int> AddAsync(Product product);
        Task<IEnumerable<Product>> SearchAsync(ProductSearchRequest request);
    }
}