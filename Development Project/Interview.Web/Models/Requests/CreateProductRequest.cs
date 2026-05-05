// EVAL: Request DTOs are separate from domain models. This allows the API contract
// to evolve independently of the domain, and validation attributes live here
// rather than polluting the domain layer.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Interview.Web.Models.Requests
{
    /// <summary>
    /// Request body for creating a new product.
    /// </summary>
    public class CreateProductRequest
    {
        /// <summary>
        /// Product name. Required.
        /// </summary>
        // RV: MaxLength matches DB column [Instances].[Products].[Name] VARCHAR(256)
        [Required(ErrorMessage = "Product name is required.")]
        [MaxLength(256, ErrorMessage = "Product name cannot exceed 256 characters.")]
        public string Name { get; set; }

        /// <summary>
        /// Product description. Required.
        /// </summary>
        [Required(ErrorMessage = "Product description is required.")]
        [MaxLength(256, ErrorMessage = "Product description cannot exceed 256 characters.")]
        public string Description { get; set; }

        /// <summary>
        /// List of product image URIs. Each entry must be a valid absolute URI.
        /// </summary>
        public List<string> ImageUris { get; set; } = new List<string>();

        /// <summary>
        /// List of valid SKUs for this product.
        /// </summary>
        public List<string> Skus { get; set; } = new List<string>();

        /// <summary>
        /// Arbitrary metadata key-value pairs (e.g., Color=Red, Brand=Nike, Size=10).
        /// Keys are limited to 64 characters, values to 512 characters.
        /// </summary>
        // EVAL: Using Dictionary allows clients to pass any metadata without
        // requiring schema changes. This meets the "arbitrary metadata" requirement.
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Category IDs to associate this product with.
        /// </summary>
        public List<int> CategoryIds { get; set; } = new List<int>();
    }
}
