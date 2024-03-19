using System.Collections.Generic;

namespace Interview.Web.DTO
{
    public class ProductCreationRequestDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProductImageUris { get; set; }
        public string ValidSkus { get; set; }
        public List<CategoriesDto> Categories { get; set; }
        public List<AttributeDto> Attributes { get; set; }
    }

    public class AttributeDto
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class CategoriesDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
