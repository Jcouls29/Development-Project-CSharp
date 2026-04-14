using System;

namespace Sparcpoint.Inventory.Models
{
    public sealed class InventoryTransaction
    {
        public int TransactionId { get; set; }
        public int ProductInstanceId { get; set; }
        public decimal Quantity { get; set; }
        public DateTime StartedTimestamp { get; set; }
        public DateTime? CompletedTimestamp { get; set; }
        public string? TypeCategory { get; set; }
    }

    public static class InventoryTransactionTypes
    {
        public const string Add = "ADD";
        public const string Remove = "REMOVE";
    }
}
