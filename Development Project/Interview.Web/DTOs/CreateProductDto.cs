using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Interview.Web.DTOs
{
    public class CreateProductDto
    {
        [Required(ErrorMessage = "{0} is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public string Description { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public string ImageUris { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public string ValidSkus { get; set; }

        public virtual List<ProductAttributeDto> Attributes { get; set; } = new List<ProductAttributeDto>();

        public virtual List<int> Categories { get; set; } = new List<int>();
    }
}
