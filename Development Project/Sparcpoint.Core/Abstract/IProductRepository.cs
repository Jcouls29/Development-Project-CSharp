using System.Collections.Generic;
using System.Threading.Tasks;
using Sparcpoint.Core.Models;

namespace Sparcpoint.Core.Abstract
{
    public interface IProductRepository
    {
        Task<int> AddProductAsync(ProductCreateRequest request);
        Task<IList<ProductSearchResult>> SearchProductsAsync(ProductSearchCriteria criteria);
    }
}