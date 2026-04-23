using System.Collections.Generic;

namespace Interview.Web.Models.Product
{
    public class CreateProductRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<CreateProductAttributeRequest> Metadata { get; set; } = new();
        public List<int> CategoryIds { get; set; } = new();
    }

    public class CreateProductAttributeRequest
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class ProductSearchRequest
    {
        public string Name { get; set; }
        public int? CategoryId { get; set; }
        public string AttributeKey { get; set; }
        public string AttributeValue { get; set; }
    }
}
