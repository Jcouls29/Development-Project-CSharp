using System;
using System.ComponentModel.DataAnnotations;

namespace Interview.Web.Models
{
    public class InventoryTransactions
    {
        [Key]
        public int TransactionId { get; set; }
        public DateTime StartedTimestamp { get; set; }
        public DateTime CompletedTimestamp { get; set; }
        public string TypeCategory { get; set; }
        public decimal Quantity { get; set; }
        public int ProductInstanceId { get; set; }
    }
}
