using Interview.Web.Contracts.Products;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Interview.Web.Services.Products
{
    public interface IProductService
    {
        Task<int> AddProductAsync(CreateProductRequest request);
        Task<IReadOnlyList<SearchProductItemResponse>> SearchProductsAsync(string searchText, string metadataKey, string metadataValue, string categoryIdsCsv);
        Task<AdjustInventoryResponse> AddInventoryAsync(int productId, AdjustInventoryRequest request);
        Task<AdjustInventoryResponse> RemoveInventoryAsync(int productId, AdjustInventoryRequest request);
        Task<decimal> GetInventoryCountAsync(int productId);
    }
}
