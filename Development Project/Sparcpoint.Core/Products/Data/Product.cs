using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Sparcpoint.Products.Data
{
    // EVAL: There's a bit of a design choice here affected by the limitations of SQL Server.
    // There's a 1:M relationship between Products, which are generally coded with an overall item number, and the item/color/size/etc that makes up a SKU
    // Realistically, an entire item should be a document but SQL server is terrible at indexing documents. Thus the compromise
    public class Product
    {
        public int ProductId { get; set; }
        public string Manufacturer { get; set; }
        public string ModelName { get; set; }
        public string Description { get; set; }
        public List<string> Categories { get; set; }

        // The raw Json string from the database gets sent here to populate the publicly available dictionary. 
        internal string CategoriesJson
        {
            set
            {
                Categories = JsonConvert.DeserializeObject<List<string>>(value);
            }
        }

        public List<InventoryItem> Items { get; set; }

        public Product()
        {
            Items = new List<InventoryItem>();
            Categories = new List<string>();
        }
    }
}
