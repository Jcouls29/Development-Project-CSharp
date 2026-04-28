using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Abstractions
{
    public interface IInventoryRepository
    {
        /// <summary>
        /// Records stock being added for a product. Quantity must be positive.
        /// Returns the new TransactionId.
        /// </summary>
        Task<int> AddAsync(int productInstanceId, decimal quantity, string typeCategory = null);

        /// <summary>
        /// Bulk add — all items processed within a single transaction.
        /// </summary>
        Task AddBatchAsync(IEnumerable<InventoryBatchItem> items);

        /// <summary>
        /// Records stock being removed. Quantity must be positive (stored as negative internally).
        /// Returns the new TransactionId.
        /// </summary>
        Task<int> RemoveAsync(int productInstanceId, decimal quantity, string typeCategory = null);

        /// <summary>
        /// Bulk remove — all items processed within a single transaction.
        /// </summary>
        Task RemoveBatchAsync(IEnumerable<InventoryBatchItem> items);

        /// <summary>
        /// EVAL: "Undo" mechanism — deleting the transaction row reverses its effect
        /// on inventory count without requiring a compensating transaction.
        /// </summary>
        Task DeleteTransactionAsync(int transactionId);

        /// <summary>
        /// Returns current net quantity for a single product.
        /// </summary>
        Task<decimal> GetCountAsync(int productInstanceId);

        /// <summary>
        /// Returns net quantities for all products matching the given filter.
        /// </summary>
        Task<IEnumerable<ProductInventoryCount>> GetCountByFilterAsync(ProductSearchFilter filter);
    }
}
