using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sparcpoint.Inventory.Models.DTOs.Requests
{
    public class CreateCategoryRequest
    {
        [Required(ErrorMessage = "Category name is required")]
        [StringLength(64, MinimumLength = 1, ErrorMessage = "Category name must be between 1 and 64 characters")]
        public string Name { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Category description is required")]
        [StringLength(256, ErrorMessage = "Description cannot exceed 256 characters")]
        public string Description { get; set; } = string.Empty;
        
        /// <summary>
        /// Optional attributes for the category.
        /// </summary>
        public Dictionary<string, string>? Attributes { get; set; }
        
        /// <summary>
        /// Parent category IDs for hierarchical structure.
        /// EVAL: Supports requirement for category hierarchies.
        /// </summary>
        public List<int>? ParentCategoryIds { get; set; }
    }
}