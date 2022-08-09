using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Sparcpoint.Models.DomainDto.Product
{
    public class ProductDto
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProductImageUri { get; set; }
        public string ValidSkus { get; set; }
        public DateTime CreatedDate { get; set; }
        public virtual ICollection<ProductAttributeDto> Attributes { get; set; } 
        public virtual ICollection<ProductCategoryDto> Categories { get; set; }
    }
}
