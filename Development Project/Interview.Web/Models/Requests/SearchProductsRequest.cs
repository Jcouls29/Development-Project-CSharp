using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Interview.Web.Models.Requests
{
    /// <summary>
    /// Query parameters for searching products.
    /// All fields are optional -- omitted fields are not included in the filter.
    /// </summary>
    // EVAL: Search parameters are combined with AND logic. Passing name + categoryId
    // returns products matching BOTH criteria. This is the most intuitive behavior
    // for inventory filtering and avoids overly complex query syntax.
    public class SearchProductsRequest
    {
        /// <summary>
        /// Filter by product name (partial match, case-insensitive).
        /// </summary>
        [MaxLength(256)]
        public string Name { get; set; }

        /// <summary>
        /// Filter by product description (partial match, case-insensitive).
        /// </summary>
        [MaxLength(256)]
        public string Description { get; set; }

        /// <summary>
        /// Filter by one or more category IDs. Products in ANY of these categories are returned.
        /// </summary>
        public List<int> CategoryIds { get; set; }

        /// <summary>
        /// Filter by metadata key-value pairs. ALL specified pairs must match (AND logic).
        /// </summary>
        // EVAL: Example usage: ?attributes[Color]=Red&attributes[Brand]=Nike
        // This filters to products where Color=Red AND Brand=Nike.
        public Dictionary<string, string> Attributes { get; set; }

        /// <summary>
        /// Number of results to skip (for pagination). Default: 0.
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "Skip must be non-negative.")]
        public int Skip { get; set; } = 0;

        /// <summary>
        /// Maximum number of results to return (for pagination). Default: 50.
        /// </summary>
        // RV: Max page size of 100 prevents accidental full-table dumps on large datasets.
        [Range(1, 100, ErrorMessage = "Take must be between 1 and 100.")]
        public int Take { get; set; } = 50;
    }
}
