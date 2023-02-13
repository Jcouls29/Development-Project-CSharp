using System.Collections.Generic;

namespace Interview.Web.Models
{
    public class CreateProductRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProductImageUris { get; set; }
        public string ValidSkus { get; set; }

        public List<ProductAttributeRequest> ProductAttributes { get; set; }
        public List<ProductCategoryRequest> Categories { get; set; }
    }
}
