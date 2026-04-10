using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Interview.Web.DTOs
{
    /// <summary>
    /// EVAL: Request DTO for creating a new category.
    /// Supports hierarchical categories via ParentCategoryIds.
    /// </summary>
    public class CreateCategoryRequest
    {
        [Required]
        [MaxLength(64)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(256)]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Arbitrary metadata as key-value pairs.
        /// </summary>
        public Dictionary<string, string> Attributes { get; set; }

        /// <summary>
        /// Parent category IDs to create a hierarchy.
        /// A category can belong to multiple parent categories.
        /// </summary>
        public List<int> ParentCategoryIds { get; set; }
    }
}
