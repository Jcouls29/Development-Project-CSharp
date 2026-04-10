using Sparcpoint.Inventory.Models;

namespace Sparcpoint.Inventory.Repositories
{
    /// <summary>
    /// EVAL: Repository interface for Product data access.
    /// Follows the Repository pattern to decouple data access from business logic.
    /// Implementations can be swapped via DI (e.g., SqlServer, InMemory for testing).
    /// </summary>
    public interface IProductRepository
    {
        Task<Product> CreateAsync(Product product);
        Task<Product?> GetByIdAsync(int instanceId);
        Task<IEnumerable<Product>> SearchAsync(ProductSearchCriteria criteria);
    }
}
