using Sparcpoint.Models;
using Sparcpoint.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sparcpoint.Abstract
{
    public interface IProductRepository
    {
        Task<int> CreateProductAsync(CreateProductRequest request);
        Task<IEnumerable<Product>> SearchProductAsync(SearchProductRequest request);
        Task<IEnumerable<Product>> GetAllProducts();
    }
}
