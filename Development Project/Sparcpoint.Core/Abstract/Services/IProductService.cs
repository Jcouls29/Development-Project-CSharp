using Sparcpoint.Models;
using Sparcpoint.Request;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sparcpoint.Abstract.Services
{
    public interface IProductService
    {
        Task<int> CreateAsync(CreateProductRequest request);
        Task<ProductDetail> GetByIdAsync(int instanceId);
        Task<List<ProductDetail>> SearchAsync(ProductSearchRequest request);
    }
}
