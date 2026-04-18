using Interview.Web.Models.Products;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Interview.Web.Repositories.Products
{
    public interface IProductRepository
    {
        Task<int> AddAsync(ProductCreateModel product);
        Task<IReadOnlyList<ProductSearchResultModel>> SearchAsync(ProductSearchRequestModel request);
    }
}
