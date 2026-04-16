using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Application.DTOs
{
    public class InventoryRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
