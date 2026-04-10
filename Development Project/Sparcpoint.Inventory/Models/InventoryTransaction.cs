namespace Sparcpoint.Inventory.Models
{
    /// <summary>
    /// EVAL: Maps to [Transactions].[InventoryTransactions].
    /// Positive Quantity = adding to inventory, Negative Quantity = removing from inventory.
    /// Individual transactions can be removed to support "undo" functionality.
    /// </summary>
    public class InventoryTransaction
    {
        public int TransactionId { get; set; }
        public int ProductInstanceId { get; set; }
        public decimal Quantity { get; set; }
        public DateTime StartedTimestamp { get; set; }
        public DateTime? CompletedTimestamp { get; set; }
        public string? TypeCategory { get; set; }
    }
}
