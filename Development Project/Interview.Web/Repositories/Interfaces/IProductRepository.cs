using System.Collections.Generic;
using System.Threading.Tasks;
using Interview.Web.Models;

namespace Interview.Web.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<int> AddProductAsync(CreateProductRequest request);
        Task<IEnumerable<ProductResponse>> SearchProductsAsync(string category = null, string attributeKey = null, string attributeValue = null);
    }
}
