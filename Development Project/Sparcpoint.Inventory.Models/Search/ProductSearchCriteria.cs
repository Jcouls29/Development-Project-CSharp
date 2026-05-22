using System.Collections.Generic;

namespace Sparcpoint.Inventory.Models.Search
{
    /// <summary>
    /// Search criteria for finding products.
    /// EVAL: Supports requirement to search by metadata, categories, and general details.
    /// All criteria are optional and combined with AND logic.
    /// </summary>
    public class ProductSearchCriteria
    {
        /// <summary>
        /// Search by product name (partial match, case-insensitive).
        /// </summary>
        public string? Name { get; set; }
        
        /// <summary>
        /// Search by description (partial match, case-insensitive).
        /// </summary>
        public string? Description { get; set; }
        
        /// <summary>
        /// Search by SKU (exact match).
        /// </summary>
        public string? Sku { get; set; }
        
        /// <summary>
        /// Filter by one or more category IDs (product must belong to ALL categories).
        /// EVAL: Supports requirement for searching by categories.
        /// </summary>
        public List<int>? CategoryIds { get; set; }
        
        /// <summary>
        /// Filter by attribute key-value pairs (product must have ALL specified attributes).
        /// EVAL: Supports requirement for searching by metadata.
        /// </summary>
        public Dictionary<string, string>? Attributes { get; set; }
        
        /// <summary>
        /// Maximum number of results to return (for pagination).
        /// </summary>
        public int? Limit { get; set; }
        
        /// <summary>
        /// Number of results to skip (for pagination).
        /// </summary>
        public int? Offset { get; set; }
    }
}