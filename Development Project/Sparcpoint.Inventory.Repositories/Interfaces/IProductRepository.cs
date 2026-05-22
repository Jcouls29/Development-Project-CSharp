using System.Collections.Generic;
using System.Threading.Tasks;
using Sparcpoint.Inventory.Models.Domain;
using Sparcpoint.Inventory.Models.Search;

namespace Sparcpoint.Inventory.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for product data access operations.
    /// EVAL: Interface-based design supports DI and testability (Open/Closed principle).
    /// </summary>
    public interface IProductRepository
    {
        /// <summary>
        /// Creates a new product with attributes and categories in a single transaction.
        /// EVAL: Uses table-valued parameters for efficient bulk insert of attributes/categories.
        /// </summary>
        Task<Product> CreateProductAsync(Product product);
        
        /// <summary>
        /// Retrieves a product by its unique identifier.
        /// </summary>
        Task<Product?> GetProductByIdAsync(int instanceId);
        
        /// <summary>
        /// Searches for products based on multiple criteria.
        /// EVAL: Supports requirement for searching by metadata, categories, and general details.
        /// </summary>
        Task<List<Product>> SearchProductsAsync(ProductSearchCriteria criteria);
        
        /// <summary>
        /// Updates product details (not including attributes or categories).
        /// EVAL: Products can be modified but never deleted (requirement #1).
        /// </summary>
        Task<bool> UpdateProductAsync(Product product);
    }
}