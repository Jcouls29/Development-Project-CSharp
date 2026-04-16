using System.Collections.Generic;

namespace Interview.Web.Models
{
    public class CreateProductRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProductImageUris { get; set; }
        public string ValidSkus { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
        public List<int> CategoryIds { get; set; }
    }
}
