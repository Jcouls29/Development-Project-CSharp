using System;
using System.Collections.Generic;
using System.Text;

namespace Sparcpoint.Inventory.Data
{
    public class ProductItem
    {
        public int ProductId { get; private set; }
        public string Manufacturer { get; set; }
        public string BrandName { get; set; }
        public string Description { get; set; }
        public List<InventoryItem> Items { get; set; }

        public ProductItem()
        {
            Items = new List<InventoryItem>();
        }
    }
}
