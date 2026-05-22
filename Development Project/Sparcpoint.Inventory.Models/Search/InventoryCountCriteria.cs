using System.Collections.Generic;

namespace Sparcpoint.Inventory.Models.Search
{
    /// <summary>
    /// Criteria for retrieving inventory counts.
    /// EVAL: Supports requirement to get inventory by product ID or metadata.
    /// </summary>
    public class InventoryCountCriteria
    {
        /// <summary>
        /// Specific product instance ID.
        /// </summary>
        public int? ProductInstanceId { get; set; }
        
        /// <summary>
        /// Filter by product attributes (e.g., get count of all "Red" products).
        /// </summary>
        public Dictionary<string, string>? ProductAttributes { get; set; }
        
        /// <summary>
        /// Filter by category IDs.
        /// </summary>
        public List<int>? CategoryIds { get; set; }
        
        /// <summary>
        /// Only include completed transactions.
        /// </summary>
        public bool OnlyCompleted { get; set; } = true;
    }
}