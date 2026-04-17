using Sparcpoint.Domain;
using Sparcpoint.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sparcpoint.Abstract.Repositories
{
    public interface IProductRepository
    {
        Task<int> AddProductAsync(CreateProductRequestDto request);

        Task<IEnumerable<Product>> SearchProductsAsync(ProductSearchRequestDto request);

    }
}
