using System;
using System.Collections.Generic;

namespace Sparcpoint.Inventory.Models.Domain
{
    /// <summary>
    /// Represents a category for organizing products hierarchically.
    /// EVAL: Supports requirement for product categorization and hierarchies.
    /// </summary>
    public class Category
    {
        public int InstanceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        
        /// <summary>
        /// Arbitrary metadata attributes for categories.
        /// EVAL: Allows categories to have custom attributes for filtering.
        /// </summary>
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
        
        /// <summary>
        /// Parent category IDs for hierarchical relationships.
        /// EVAL: Supports multi-level category hierarchies.
        /// </summary>
        public List<int> ParentCategoryIds { get; set; } = new List<int>();
        
        public DateTime CreatedTimestamp { get; set; }
    }
}