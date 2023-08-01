using System;
using System.Collections.Generic;

namespace Interview.Web.Dtos
{
    public class ProductCreateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProductImageUris { get; set; }
        public string ValidSkus { get; set; }
        public DateTime CreatedTimeStamp { get; set; }
        public List<CategoryCreateDto> Categories { get; set; } = new List<CategoryCreateDto>();
        public List<string> Attributes { get; set; } = new List<string>();
    }
}
