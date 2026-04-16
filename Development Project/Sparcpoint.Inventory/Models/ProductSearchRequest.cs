using System.Collections.Generic;

namespace Sparcpoint.Inventory.Models
{
    public class ProductSearchRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public List<int> CategoryIds { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 25;
    }
}
