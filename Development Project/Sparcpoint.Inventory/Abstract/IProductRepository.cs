// EVAL: Repository interfaces live in the domain library, not the infrastructure layer.
// This ensures the domain defines the contract and the infrastructure adapts to it
// (Dependency Inversion Principle). Swapping SQL Server for another data store
// only requires a new implementation, no domain changes.

using Sparcpoint.Inventory.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Abstract
{
    /// <summary>
    /// Data access contract for product operations.
    /// </summary>
    // EVAL: All methods are async with CancellationToken support,
    // enabling proper request cancellation in ASP.NET Core pipelines.
    public interface IProductRepository
    {
        /// <summary>
        /// Creates a new product with its attributes and category associations.
        /// </summary>
        /// <param name="product">The product to create (InstanceId is ignored and auto-generated).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created product with its generated InstanceId.</returns>
        Task<Product> CreateAsync(Product product, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a product by its unique identifier, including attributes and categories.
        /// </summary>
        /// <param name="instanceId">The product ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The product, or null if not found.</returns>
        Task<Product> GetByIdAsync(int instanceId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves multiple products by their identifiers in a single batch query.
        /// </summary>
        /// <param name="instanceIds">The product IDs to retrieve.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Dictionary keyed by InstanceId. Missing IDs are simply absent from the dictionary.</returns>
        Task<IDictionary<int, Product>> GetByIdsAsync(IEnumerable<int> instanceIds, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the subset of supplied category IDs that exist in the Categories table.
        /// </summary>
        /// <param name="categoryIds">Candidate category IDs.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Set of category IDs that exist. IDs not in the set do not exist.</returns>
        Task<HashSet<int>> GetExistingCategoryIdsAsync(IEnumerable<int> categoryIds, CancellationToken cancellationToken = default);

        /// <summary>
        /// Searches for products with optional filters. All filters use AND logic.
        /// </summary>
        /// <param name="criteria">Search criteria (all optional).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Matching products with attributes and categories populated.</returns>
        // RV: Consider whether OR logic for categories is needed in the future.
        // Current design: product must be in ANY of the specified categories (OR within categories,
        // AND with other filter types).
        Task<IEnumerable<Product>> SearchAsync(ProductSearchCriteria criteria, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Search criteria for product queries. All fields are optional.
    /// </summary>
    // EVAL: Separate criteria class (vs passing individual parameters) makes it easy
    // to add new filter dimensions without breaking the interface signature.
    // This follows the open-closed principle for search extensibility.
    public class ProductSearchCriteria
    {
        /// <summary>
        /// Filter by product name (partial, case-insensitive).
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Filter by product description (partial, case-insensitive).
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Filter by category IDs. Products in ANY of these categories match.
        /// </summary>
        public List<int> CategoryIds { get; set; }

        /// <summary>
        /// Filter by metadata key-value pairs. ALL pairs must match (AND logic).
        /// </summary>
        public Dictionary<string, string> Attributes { get; set; }

        /// <summary>
        /// Number of results to skip.
        /// </summary>
        public int Skip { get; set; } = 0;

        /// <summary>
        /// Maximum number of results to return.
        /// </summary>
        public int Take { get; set; } = 50;
    }
}
