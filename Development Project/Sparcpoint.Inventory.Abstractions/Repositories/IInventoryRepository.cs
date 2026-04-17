using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sparcpoint.Inventory.Models;

namespace Sparcpoint.Inventory.Repositories
{
    /// <summary>
    /// EVAL: Inventory operations. The contract covers the 4 sub-requirements:
    ///  (3) add inventory, (4) remove inventory, (5) counts by
    ///  product/metadata, (6) revert a single transaction (undo).
    /// Bulk operations accept a list of adjustments for a single round-trip to SQL.
    /// </summary>
    public interface IInventoryRepository
    {
        /// <summary>Records one or more transactions (positive or negative) in a single atomic operation.</summary>
        Task<IReadOnlyList<int>> RecordAsync(IEnumerable<InventoryAdjustment> adjustments, CancellationToken cancellationToken = default);

        /// <summary>Reverts a single transaction via soft-delete (sets CompletedTimestamp = NULL).</summary>
        Task<bool> RevertAsync(int transactionId, CancellationToken cancellationToken = default);

        /// <summary>Returns inventory counts grouped by product according to the query filters.</summary>
        Task<IReadOnlyList<InventoryCountResult>> GetCountsAsync(InventoryCountQuery query, CancellationToken cancellationToken = default);

        /// <summary>Lists transactions for a product. Useful for auditing and to know what can be reverted.</summary>
        Task<IReadOnlyList<InventoryTransaction>> ListTransactionsAsync(int productInstanceId, CancellationToken cancellationToken = default);
    }
}
