using System;
using Newtonsoft.Json;

namespace Interview.Web.Model
{
    public class InventoryTransactions
    {
        public int TransactionId { get; set; }
        public int ProductInstanceId { get; set; }
        public decimal Quantity { get; set; }
        public DateTime StartedTimestamp { get; set; }
        public DateTime CompletedTimestamp { get; set; }
        public string TypeCategory { get; set; }

    }
}
