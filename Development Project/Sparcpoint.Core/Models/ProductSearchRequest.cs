using System.Collections.Generic;

namespace Sparcpoint.Models
{
    public class ProductSearchRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public List<int> CategoryIds { get; set; }
    }
}
