using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Sparcpoint.Inventory.Data
{
    public class InventoryItem
    {
        private Dictionary<string,string> _attributes = new Dictionary<string,string>();
        public string Sku { get; private set; }
        public int QuantityOnHand { get; set; }
        public string AttributesJson { 
            set { 
                _attributes = JsonConvert.DeserializeObject<Dictionary<string,string>>(value);
            } 
        }
        public Dictionary<string, string> Attributes { 
            get { return _attributes; }
        }

        public InventoryItem()
        {
        }
    }
}
