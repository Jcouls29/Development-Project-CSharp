using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.models
{
    public class ProductInventoryModel
    {
        public string TransactionId { get; set; }
        public string ProductInstanceId { get; set; }
        public int Quantity { get; set; }
        public string StartedTimestamp { get; set; }
        public string CompletedTimestamp { get; set; }
        public string TypeCategory { get; set; }
    }
}
