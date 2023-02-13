﻿using Dapper.Contrib.Extensions;

namespace Sparcpoint.Inventory.Models
{
    [Table("[Transactions].[InventoryTransactions]")]
    public class InventoryTransaction
    {
        [Key]
        public int TransactionId { get; set; }

        public int ProductInstanceId { get; set; }
        public int Quantity { get; set; }
        public DateTime StartedTimestamp { get; set; }
        public DateTime CompletedTimestamp { get; set; }
        public string TypeCategory { get; set; }
    }
}