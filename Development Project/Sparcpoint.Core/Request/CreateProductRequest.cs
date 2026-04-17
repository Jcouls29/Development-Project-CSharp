using System.Collections.Generic;

namespace Sparcpoint.Request
{
    public class CreateProductRequest
    {        
        public string Name { get; set; }             
        public string Description { get; set; }
        public List<string> ProductImageUris { get; set; }
        public List<string> ValidSkus { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public List<int> CategoryIds { get; set; }
    }
}
