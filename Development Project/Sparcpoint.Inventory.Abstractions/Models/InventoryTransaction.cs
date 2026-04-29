using System;

namespace Sparcpoint.Inventory.Abstractions
{
    /// <summary>
    /// Represents a single inventory movement.
    /// Positive Quantity = stock added; Negative Quantity = stock removed.
    /// Deleting a transaction row is the "undo" mechanism (requirement 6).
    /// </summary>
    public class InventoryTransaction
    {
        public int TransactionId { get; set; }
        public int ProductInstanceId { get; set; }
        public decimal Quantity { get; set; }
        public DateTime StartedTimestamp { get; set; }
        public DateTime? CompletedTimestamp { get; set; }
        public string TypeCategory { get; set; }
    }
}
