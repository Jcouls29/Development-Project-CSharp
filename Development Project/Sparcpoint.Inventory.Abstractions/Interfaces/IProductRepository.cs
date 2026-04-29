using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Abstractions
{
    /// <summary>
    /// EVAL: Repository pattern - isolates data access behind a clean interface,
    /// allowing SQL Server to be swapped for another store without touching callers.
    /// </summary>
    public interface IProductRepository
    {
        /// <summary>
        /// Adds a new product with its metadata and category assignments.
        /// Returns the new product's InstanceId.
        /// </summary>
        Task<int> AddAsync(CreateProductRequest request);

        /// <summary>
        /// Searches products by any combination of name, attributes, and categories.
        /// All supplied filter fields are applied with AND semantics.
        /// </summary>
        Task<IEnumerable<Product>> SearchAsync(ProductSearchFilter filter);
    }
}
