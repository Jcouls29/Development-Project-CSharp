using System.Collections.Generic;
using System.Threading.Tasks;
using Sparcpoint.Inventory.Models.DTOs.Requests;
using Sparcpoint.Inventory.Models.DTOs.Responses;
using Sparcpoint.Inventory.Models.Search;

namespace Sparcpoint.Inventory.Services.Interfaces
{
    /// <summary>
    /// Service interface for product business logic.
    /// EVAL: Separates business logic from data access for better testability and maintainability.
    /// </summary>
    public interface IProductService
    {
        Task<ProductResponse> CreateProductAsync(CreateProductRequest request);
        Task<ProductResponse?> GetProductByIdAsync(int instanceId);
        Task<List<ProductResponse>> SearchProductsAsync(ProductSearchCriteria criteria);
        Task<InventoryCountResponse?> GetProductInventoryAsync(int instanceId);
    }
}