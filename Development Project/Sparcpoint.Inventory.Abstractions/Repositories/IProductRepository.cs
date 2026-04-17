using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sparcpoint.Inventory.Models;

namespace Sparcpoint.Inventory.Repositories
{
    /// <summary>
    /// EVAL: Primary abstraction for reading/writing products.
    /// Does not include Delete (requirement #1: products are never removed).
    /// All methods are async — synchronous overloads are available via extension if a
    /// legacy caller needs them (see SyncRepositoryExtensions).
    /// </summary>
    public interface IProductRepository
    {
        /// <summary>Persists a new product together with its attributes and categories. Returns the assigned InstanceId.</summary>
        Task<int> AddAsync(Product product, CancellationToken cancellationToken = default);

        /// <summary>Retrieves a product by its InstanceId or null if it does not exist.</summary>
        Task<Product> GetByIdAsync(int instanceId, CancellationToken cancellationToken = default);

        /// <summary>Combined search by name, metadata and categories.</summary>
        Task<IReadOnlyList<Product>> SearchAsync(ProductSearchCriteria criteria, CancellationToken cancellationToken = default);
    }
}
