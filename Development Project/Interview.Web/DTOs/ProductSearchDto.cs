using System.Collections.Generic;

namespace Interview.Web.DTOs
{
    public class ProductSearchDto
    {
        public string SearchText { get; set; }

        public List<ProductAttributeDto> Attributes { get; set; }

        public List<int> CategoryIds { get; set; }
    }
}
