using Interview.Web.Dtos.Requests;
using Sparcpoint.Inventory.Application.Dtos.Requests;

namespace Sparcpoint.Inventory.Application.Interfaces
{
    public interface IProductService
    {
        Task<int> CreateAsync(CreateProductRequest request);
        Task<List<int>> SearchAsync(SearchProductsRequest request);
    }
}
