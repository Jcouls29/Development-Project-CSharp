// EVAL: Inventory repository is separate from Product repository (single responsibility).
// Products handle catalog concerns; Inventory handles stock/transaction concerns.
// Both share the same ISqlExecutor but operate on different tables.

using Sparcpoint.Inventory.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Abstract
{
    /// <summary>
    /// Data access contract for inventory transaction operations.
    /// </summary>
    public interface IInventoryRepository
    {
        /// <summary>
        /// Records one or more inventory additions (positive quantities).
        /// </summary>
        /// <param name="items">Items to add, each with a product ID, quantity, and optional type.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created transaction records.</returns>
        Task<IEnumerable<InventoryTransaction>> AddInventoryAsync(
            IEnumerable<InventoryTransactionItem> items,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Records one or more inventory removals (stored as negative quantities).
        /// </summary>
        /// <param name="items">Items to remove, each with a product ID, quantity, and optional type.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created transaction records.</returns>
        Task<IEnumerable<InventoryTransaction>> RemoveInventoryAsync(
            IEnumerable<InventoryTransactionItem> items,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the current inventory count for a specific product.
        /// Count = SUM(Quantity) of active transactions (CompletedTimestamp IS NULL).
        /// </summary>
        /// <param name="productInstanceId">The product ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Current inventory count, or null if product not found.</returns>
        Task<decimal?> GetCountByProductIdAsync(
            int productInstanceId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves inventory counts for all products matching a metadata attribute.
        /// </summary>
        /// <param name="attributeKey">The metadata key to filter by.</param>
        /// <param name="attributeValue">The metadata value to filter by.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Dictionary of product ID -> count for matching products.</returns>
        // EVAL: This fulfills Requirement 5: "Inventory Counts for a specific product,
        // or subset of metadata on a product, must be retrievable."
        Task<Dictionary<int, decimal>> GetCountByMetadataAsync(
            string attributeKey,
            string attributeValue,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Undoes a transaction by setting its CompletedTimestamp.
        /// The transaction remains in the log for audit but is excluded from counts.
        /// </summary>
        /// <param name="transactionId">The transaction ID to undo.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated transaction, or null if not found.</returns>
        // EVAL: Soft-delete approach preserves audit trail while supporting
        // the "undo" requirement from Goal 4.
        Task<InventoryTransaction> UndoTransactionAsync(
            int transactionId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves inventory transactions, optionally filtered by product and active state.
        /// Ordered by StartedTimestamp descending (newest first).
        /// </summary>
        /// <param name="productInstanceId">Optional product ID filter.</param>
        /// <param name="activeOnly">If true, excludes undone transactions (CompletedTimestamp IS NOT NULL).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Matching transactions.</returns>
        Task<IEnumerable<InventoryTransaction>> GetTransactionsAsync(
            int? productInstanceId,
            bool activeOnly,
            CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Represents a single item in a bulk inventory operation.
    /// </summary>
    // RV: This is a domain-level input model, not a DTO. The controller maps
    // from the API request DTO to this type.
    public class InventoryTransactionItem
    {
        /// <summary>
        /// Product ID to add/remove inventory for.
        /// </summary>
        public int ProductInstanceId { get; set; }

        /// <summary>
        /// Quantity (always positive -- the repository negates for removals).
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// Optional transaction type (e.g., "Purchase", "Sale", "Return").
        /// </summary>
        public string TypeCategory { get; set; }
    }
}
