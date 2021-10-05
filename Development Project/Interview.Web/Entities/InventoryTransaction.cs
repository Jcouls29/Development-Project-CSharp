using System;
using System.Collections.Generic;

#nullable disable

namespace Interview.Web.Entities
{
    // EVAL: Entity that represents the InventoryTransaction table.
    public partial class InventoryTransaction
    {
        public int TransactionId { get; set; }
        public int ProductInstanceId { get; set; }
        public decimal Quantity { get; set; }
        public DateTime StartedTimestamp { get; set; }
        public DateTime? CompletedTimestamp { get; set; }
        public string TypeCategory { get; set; }

        public virtual Product ProductInstance { get; set; }
    }
}
