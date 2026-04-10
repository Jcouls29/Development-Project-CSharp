using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Interview.Web.DTOs
{
    /// <summary>
    /// EVAL: Request DTO for creating a new product.
    /// Uses DataAnnotations for basic validation at the API boundary.
    /// </summary>
    public class CreateProductRequest
    {
        [Required]
        [MaxLength(256)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(256)]
        public string Description { get; set; } = string.Empty;

        public List<string>? ProductImageUris { get; set; }

        public List<string>? ValidSkus { get; set; }

        /// <summary>
        /// Arbitrary metadata as key-value pairs (e.g., Color: "Red", Brand: "Acme").
        /// </summary>
        public Dictionary<string, string>? Attributes { get; set; }

        /// <summary>
        /// Category IDs to associate with this product.
        /// </summary>
        public List<int>? CategoryIds { get; set; }
    }
}
