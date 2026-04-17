using System;

namespace Sparcpoint.Inventory.Models
{
    /// <summary>
    /// EVAL: An inventory movement. Quantity &gt; 0 = add, &lt; 0 = remove.
    /// CompletedTimestamp NULL indicates that the transaction was reverted (undo)
    /// — we do NOT delete rows in order to preserve the audit trail.
    /// Mirrors [Transactions].[InventoryTransactions].
    /// </summary>
    public sealed class InventoryTransaction
    {
        public int TransactionId { get; set; }
        public int ProductInstanceId { get; set; }
        public decimal Quantity { get; set; }
        public DateTime StartedTimestamp { get; set; }
        public DateTime? CompletedTimestamp { get; set; }

        /// <summary>
        /// Free-form type: "ADD", "REMOVE", "ADJUST", "RETURN", etc. Configurable per client.
        /// </summary>
        public string TypeCategory { get; set; }
    }
}
