using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Domain.Requestes
{
    public class InventoryTransactionRequest
    {
        public int ProductInstanceId { get; set; }
        public int Quantity { get; set; }
    }
}
