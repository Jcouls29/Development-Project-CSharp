using System.Collections.Generic;

namespace Interview.Web.Contracts.Products
{
    public class CreateProductRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> ProductImageUris { get; set; } = new List<string>();
        public List<string> ValidSkus { get; set; } = new List<string>();
        public List<CreateProductAttributeRequest> Metadata { get; set; } = new List<CreateProductAttributeRequest>();
        public List<int> CategoryIds { get; set; } = new List<int>();
    }

    public class CreateProductAttributeRequest
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
