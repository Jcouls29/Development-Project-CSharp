using System.Collections.Generic;

namespace Sparcpoint.Inventory.Models.Requests
{
    public class ProductSearchRequest
    {
        public string Name { get; set; }
        public IEnumerable<int> CategoryIds { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
    }
}
