using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Model
{
    public class TransactionsAttributes
    {

        public TransactionsAttributes() { }

        #region ProductsAttributes

        public int TransactionId { get; set; }
        public int ProductInstanceId { get; set; }
        public string Quantity { get; set; }
        public DateTime StartedTimestamp { get; set; }
        public DateTime CompletedTimestamp { get; set; }
        public string TypeCategory { get; set; }

        #endregion
    }
}
