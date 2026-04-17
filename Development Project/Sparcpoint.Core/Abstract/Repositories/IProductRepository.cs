using Sparcpoint.Models;
using Sparcpoint.Request;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sparcpoint.Abstract.Repositories
{
    public interface IProductRepository
    {
        Task<int> CreateAsync(CreateProductRequest request);
        Task<ProductDetail> GetByIdAsync(int instanceId);
        Task<List<ProductDetail>> SearchAsync(Models.ProductSearchRequest request);
    }
}
