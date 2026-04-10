using Sparcpoint.Inventory.Models;

namespace Sparcpoint.Inventory.Services
{
    /// <summary>
    /// EVAL: Service interface for product business logic.
    /// Separates business rules (validation, orchestration) from data access.
    /// </summary>
    public interface IProductService
    {
        Task<Product> CreateProductAsync(Product product);
        Task<Product?> GetProductByIdAsync(int instanceId);
        Task<IEnumerable<Product>> SearchProductsAsync(ProductSearchCriteria criteria);
    }
}
