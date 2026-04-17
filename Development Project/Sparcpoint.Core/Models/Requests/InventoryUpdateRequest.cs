using System.Collections.Generic;

namespace Sparcpoint.Models.Requests
{
    public class InventoryUpdateRequest
    {
        public List<int> ProductInstanceIds { get; set; } = new List<int>();
        public decimal Quantity { get; set; }
    }
}
