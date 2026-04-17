using System.Collections.Generic;

namespace Sparcpoint.DTOs
{
    public class CreateProductRequestDto
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string ImageUris { get; set; }

        public string Skus { get; set; }

        public List<ProductAttributeRequestDto> Attributes { get; set; }

        public List<int> CategoryIds { get; set; }
    }
}
