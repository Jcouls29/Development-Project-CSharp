using System;

namespace Sparcpoint.Inventory.Models.Domain
{
    /// <summary>
    /// Represents an inventory transaction (add, remove, adjustment).
    /// EVAL: Supports requirement for tracking individual transactions with ability to undo.
    /// </summary>
    public class InventoryTransaction
    {
        public int TransactionId { get; set; }
        public int ProductInstanceId { get; set; }
        
        /// <summary>
        /// Quantity change. Positive for additions, negative for removals.
        /// EVAL: Using DECIMAL(19,6) to support fractional quantities (e.g., liquids, bulk items).
        /// </summary>
        public decimal Quantity { get; set; }
        
        public DateTime StartedTimestamp { get; set; }
        
        /// <summary>
        /// When null, transaction is pending. When set, transaction is completed.
        /// EVAL: Allows for multi-step transaction workflows if needed.
        /// </summary>
        public DateTime? CompletedTimestamp { get; set; }
        
        /// <summary>
        /// Type of transaction: "ADD", "REMOVE", "ADJUSTMENT", etc.
        /// EVAL: Extensible design for future transaction types.
        /// </summary>
        public string? TypeCategory { get; set; }
        
        /// <summary>
        /// Associated product name (from join query).
        /// </summary>
        public string? ProductName { get; set; }
    }
}