using System.Collections.Generic;

namespace Sparcpoint.Core.Models
{
    public class InventoryItem
    {
        public List<ProductUpdate> Products { get; set; }
    }

    public class ProductUpdate
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}