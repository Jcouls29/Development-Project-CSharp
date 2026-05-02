using Interview.Web.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interview.Web.Data
{
    public interface IInventoryRepository
    {
        Task<ProductResponse> CreateProductAsync(CreateProductRequest request);
        Task<ProductResponse> GetProductAsync(int productId);
        Task<ProductResponse> UpdateProductAsync(int productId, UpdateProductRequest request);
        Task<IReadOnlyList<ProductResponse>> SearchProductsAsync(SearchProductsRequest request);
        Task<CategoryResponse> CreateCategoryAsync(CreateCategoryRequest request);
        Task<IReadOnlyList<CategoryResponse>> GetCategoriesAsync();
        Task<IReadOnlyList<InventoryTransactionResponse>> AddInventoryAsync(InventoryAdjustmentRequest request);
        Task<IReadOnlyList<InventoryTransactionResponse>> RemoveInventoryAsync(InventoryAdjustmentRequest request);
        Task<bool> DeleteInventoryTransactionAsync(int transactionId);
        Task<InventoryCountResponse> GetInventoryCountsAsync(InventoryCountRequest request);
    }
}
