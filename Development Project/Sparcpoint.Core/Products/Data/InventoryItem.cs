using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Sparcpoint.Products.Data
{
    public class InventoryItem
    {
        private Dictionary<string, string> _attributes = new Dictionary<string, string>();
        public int ProductId { get; set; }
        public string Sku { get; set; }
        public int QuantityOnHand { get; set; }

        // The raw Json string from the database gets sent here to populate the publicly available dictionary. 
        public string AttributesJson => JsonConvert.SerializeObject(_attributes);

        // Public access to the inventory item attributes/metadata. This gives a degree of type safety without needing a flood of class overloads
        public Dictionary<string, string> Attributes
        {
            set
            {
                _attributes = value;
            }
            get
            {
                return _attributes;
            }
        }

        public InventoryItem()
        {
        }
    }
}
