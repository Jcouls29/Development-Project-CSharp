using System.Collections.Generic;

namespace Sparcpoint.Inventory.Models
{
    public class ProductSearchFilter
    {
        public string Name { get; set; }
        public int[] CategoryIds { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
    }
}
