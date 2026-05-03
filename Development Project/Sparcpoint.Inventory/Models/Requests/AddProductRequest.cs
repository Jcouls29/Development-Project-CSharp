using System.Collections.Generic;

namespace Sparcpoint.Inventory.Models.Requests
{
    public class AddProductRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProductImageUris { get; set; }
        public string ValidSkus { get; set; }
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
        public IEnumerable<int> CategoryIds { get; set; } = new List<int>();
    }
}
