using System.ComponentModel.DataAnnotations;

namespace Sparcpoint.Inventory.Abstractions
{
    /// <summary>
    /// Request body for a single-product inventory adjustment (add or remove).
    /// </summary>
    public class InventoryAdjustRequest
    {
        /// <summary>
        /// Must be a positive value regardless of operation direction.
        /// The direction (add vs. remove) is determined by the endpoint called.
        /// </summary>
        [Range(0.001, double.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// Optional classification for this transaction (e.g. "SALE", "RETURN", "ADJUSTMENT").
        /// Maps to Transactions.InventoryTransactions.TypeCategory.
        /// </summary>
        public string TypeCategory { get; set; }
    }
}
