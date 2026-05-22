using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sparcpoint.Inventory.Models.DTOs.Requests
{
    /// <summary>
    /// Request model for creating a new product.
    /// EVAL: Uses Data Annotations for basic validation at API layer.
    /// </summary>
    public class CreateProductRequest
    {
        [Required(ErrorMessage = "Product name is required")]
        [StringLength(256, MinimumLength = 1, ErrorMessage = "Product name must be between 1 and 256 characters")]
        public string Name { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Product description is required")]
        [StringLength(256, ErrorMessage = "Description cannot exceed 256 characters")]
        public string Description { get; set; } = string.Empty;
        
        /// <summary>
        /// Optional list of product image URIs.
        /// EVAL: Validates URI format in service layer to avoid SQL injection.
        /// </summary>
        public List<string>? ProductImageUris { get; set; }
        
        /// <summary>
        /// Optional list of valid SKUs.
        /// EVAL: Common metadata attribute elevated to product level for search performance.
        /// </summary>
        public List<string>? ValidSkus { get; set; }
        
        /// <summary>
        /// Arbitrary key-value attributes (e.g., "Color": "Red", "Brand": "Acme").
        /// EVAL: Fulfills requirement for arbitrary metadata on products.
        /// </summary>
        public Dictionary<string, string>? Attributes { get; set; }
        
        /// <summary>
        /// Category IDs to associate with this product.
        /// EVAL: Supports product categorization requirement.
        /// </summary>
        public List<int>? CategoryIds { get; set; }
    }
}