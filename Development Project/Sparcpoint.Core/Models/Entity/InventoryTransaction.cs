using System;
using System.Collections.Generic;
using System.Text;

namespace Sparcpoint.Models.Entity
{
    public class InventoryTransaction
    {
        public int TransactionID { get; set; }
        public int ProductInstanceID { get; set; }
        public decimal Quantity { get; set; }
        public DateTime StartedTimestamp { get; set; }
        public DateTime? CompletedTimestamp { get; set; }
        public string TypeCategory { get; set; }
    }
}
