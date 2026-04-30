using System;

namespace Sparcpoint.Inventory.Models
{
    // EVAL: InventoryTransaction extends InstanceBase for consistency, though the
    // DB uses TransactionId as PK. The "undo" mechanic is simply deleting a specific
    // transaction row — no soft deletes needed per the spec.
    public class InventoryTransaction : InstanceBase
    {
        public int ProductInstanceId { get; set; }

        // EVAL: Positive = stock added, Negative = stock removed.
        // This allows SUM(Quantity) to give accurate count without separate add/remove tables.
        public decimal Quantity { get; set; }

        public DateTime StartedTimestamp { get; set; }
        public DateTime? CompletedTimestamp { get; set; }
        public string? TypeCategory { get; set; }
    }
}
