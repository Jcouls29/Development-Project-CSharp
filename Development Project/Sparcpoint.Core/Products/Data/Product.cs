using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        private List<string> _categories = new List<string>();
        public int ProductId { get; set; }
        public string Manufacturer { get; set; }
        public string ModelName { get; set; }
        public string Description { get; set; }
        public string CategoriesJson => JsonConvert.SerializeObject(_categories);

        // The raw Json string from the database gets sent here to populate the publicly available dictionary. 
        public List<string> Categories
        {
            set
            {
                _categories = value;
            }
            get
            {
                return _categories;
            }
        }

        public List<InventoryItem> Items { get; set; }

        public Product()
        {
            Items = new List<InventoryItem>();
        }
    }
}
