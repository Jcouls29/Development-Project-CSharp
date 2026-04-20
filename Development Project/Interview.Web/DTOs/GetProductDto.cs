using System;
using System.Collections.Generic;

namespace Interview.Web.DTOs
{
    public class GetProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUris { get; set; }
        public string ValidSkus { get; set; }
        public DateTime CreatedTimestamp { get; set; }

        public virtual List<ProductAttributeDto> Attributes { get; set; }

        public virtual List<CategoryDto> Categories { get; set; }
    }
}
