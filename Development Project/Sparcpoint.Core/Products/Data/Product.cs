using System;
using System.Collections.Generic;
using System.Text;

namespace Sparcpoint.Products.Data
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Manufacturer { get; set; }
        public string ModelName { get; set; }
        public string Description { get; set; }
        public List<InventoryItem> Items { get; set; }

        public Product()
        {
            Items = new List<InventoryItem>();
        }
    }
}
