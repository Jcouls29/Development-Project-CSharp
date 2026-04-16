using Sparcpoint.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sparcpoint
{
    public interface IProductService
    {
        Task<int> CreateAsync(Product product);
        Task<IEnumerable<Product>> SearchAsync(ProductSearchRequest request);
        Task<IEnumerable<Product>> GetAll();
    }
}
