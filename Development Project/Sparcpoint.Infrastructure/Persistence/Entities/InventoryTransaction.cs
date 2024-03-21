﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Sparcpoint.Infrastructure.Persistence.Entities
{
    public class InventoryTransaction
    {
        public int TransactionId { get; set; }
        public int ProductInstanceId { get; set; }
        public decimal Quantity { get; set; }
        public DateTime StartedTimestamp { get; set; }
        public DateTime? CompletedTimestamp { get; set; }
        public string TypeCategory { get; set; }

        // Navigation property
        public Product Product { get; set; }
    }
}
