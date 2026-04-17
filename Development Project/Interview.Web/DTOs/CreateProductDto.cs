using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Interview.Web.DTOs
{
    public class CreateProductDto
    {
        [Required(ErrorMessage = "Name required.")]
        [StringLength(256, ErrorMessage = "The name cannot exceed 256 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description required.")]
        [StringLength(256, ErrorMessage = "The description cannot exceed 256 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "ImageUris required.")]
        public string ImageUris { get; set; }

        [Required(ErrorMessage = "Skus required.")]
        public string Skus { get; set; }

        public List<ProductAttributeDto> Attributes { get; set; }

        public List<int> CategoryIds { get; set; }
    }
}
