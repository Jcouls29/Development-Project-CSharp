// <auto-generated>
// ReSharper disable All

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sparkpoint.Data
{
    // InventoryTransactions
    public class InventoryTransaction
    {
        public int TransactionId { get; set; } // TransactionId (Primary key)
        public int ProductInstanceId { get; set; } // ProductInstanceId
        public decimal Quantity { get; set; } // Quantity
        public DateTime StartedTimestamp { get; set; } // StartedTimestamp
        public DateTime? CompletedTimestamp { get; set; } // CompletedTimestamp
        public string TypeCategory { get; set; } // TypeCategory (length: 32)

        // Foreign keys

        /// <summary>
        /// Parent Product pointed by [InventoryTransactions].([ProductInstanceId]) (FK_InventoryTransactions_Products)
        /// </summary>
        public Product Product { get; set; } // FK_InventoryTransactions_Products

        public InventoryTransaction()
        {
            StartedTimestamp = DateTime.UtcNow;
        }
    }

}
// </auto-generated>