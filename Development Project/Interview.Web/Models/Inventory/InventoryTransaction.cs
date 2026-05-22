using System;

namespace Interview.Web.Models
{
    public class InventoryTransaction
    {
        public Guid TransactionId { get; set; } = Guid.NewGuid();
        public Guid ProductId { get; set; }

        public decimal Quantity { get; set; }
        public InventoryTransactionType Type { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public enum InventoryTransactionType
    {
        Adjustment,
        Purchase,
        Sale
    }

}

