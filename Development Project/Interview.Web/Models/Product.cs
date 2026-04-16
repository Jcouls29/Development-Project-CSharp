using System.Collections.Generic;

namespace Interview.Web.Models
{
    public class CreateProductRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProductImageUris { get; set; }
        public string ValidSkus { get; set; }
        public List<int> CategoryIds { get; set; } = new List<int>();
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
    }

    public class ProductResponse
    {
        public int InstanceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProductImageUris { get; set; }
        public string ValidSkus { get; set; }
        public List<string> Categories { get; set; } = new List<string>();
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
    }
}
