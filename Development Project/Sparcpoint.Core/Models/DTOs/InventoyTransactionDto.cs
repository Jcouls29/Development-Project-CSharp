using System;
using System.Collections.Generic;
using System.Text;

namespace Sparcpoint.Models.DTOs
{
    public class InventoyTransactionDto
    {
        public int TransactionId { get; private set; }
        public int ProductInstanceId { get; private set; }
        public decimal Quantity { get; private set; } = 0;
        public DateTime StartedTimestamp { get; private set; }
        public DateTime? CompletedTimestamp { get; private set; }
        public string TypeCategory { get; private set; }
    }
}
