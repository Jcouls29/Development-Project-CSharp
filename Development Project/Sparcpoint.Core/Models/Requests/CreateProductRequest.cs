using System.Collections.Generic;

namespace Sparcpoint.Models.Requests
{
    public class CreateProductRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProductImageUris { get; set; }
        public string ValidSkus { get; set; }
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
        public List<int> CategoryIds { get; set; } = new List<int>();
    }
}
