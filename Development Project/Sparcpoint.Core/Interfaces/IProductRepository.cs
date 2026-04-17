using Sparcpoint.Models;
using Sparcpoint.Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sparcpoint.Interfaces
{
    public interface IProductRepository
    {
        Task<int> CreateAsync(CreateProductRequest request);

        Task<Product> GetByIdAsync(int instanceId);

        Task<IEnumerable<Product>> SearchAsync(ProductSearchRequest request);
    }
}
