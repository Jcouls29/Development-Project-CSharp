using System.Collections.Generic;

namespace Sparcpoint.DTOs
{
    public class ProductSearchRequestDto
    {
        public string SearchText { get; set; }

        public List<ProductAttributeRequestDto> Attributes { get; set; }

        public List<int> CategoryIds { get; set; }

    }
}
