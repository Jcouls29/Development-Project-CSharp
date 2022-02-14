using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Domain.Instance.Entities
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
