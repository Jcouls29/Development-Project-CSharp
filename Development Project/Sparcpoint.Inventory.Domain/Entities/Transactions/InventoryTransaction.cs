using Sparcpoint.Inventory.Domain.Entities.Instances;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sparcpoint.Inventory.Domain.Entities.Transactions
{
    public class InventoryTransaction : TransactionEntity
    {
        public int ProductInstanceId { get; set; } = int.MinValue;
        public decimal Quantity { get; set; } = decimal.Zero;
        public DateTime StartedTimestamp { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedTimestamp { get; set; }
        public string TypeCategory { get; set; } = string.Empty;

        public Product Product { get; set; } = new Product();
    }
}
