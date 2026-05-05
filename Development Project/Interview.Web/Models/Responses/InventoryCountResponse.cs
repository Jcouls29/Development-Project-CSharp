using System.Collections.Generic;

namespace Interview.Web.Models.Responses
{
    /// <summary>
    /// API response for inventory count queries.
    /// </summary>
    public class InventoryCountResponse
    {
        /// <summary>
        /// Individual product inventory counts.
        /// </summary>
        public List<ProductInventoryCount> Items { get; set; } = new List<ProductInventoryCount>();

        /// <summary>
        /// Total count across all returned items.
        /// </summary>
        // EV: Consider whether a grand total across products is useful
        // or potentially misleading (summing different product units).
        public decimal TotalCount { get; set; }
    }

    /// <summary>
    /// Inventory count for a single product.
    /// </summary>
    public class ProductInventoryCount
    {
        /// <summary>
        /// Product identifier.
        /// </summary>
        public int ProductInstanceId { get; set; }

        /// <summary>
        /// Product name for display convenience.
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Current inventory count (SUM of active transaction quantities).
        /// </summary>
        // EVAL: Count = SUM(Quantity) WHERE CompletedTimestamp IS NULL.
        // Positive = in stock, zero = out of stock, negative = oversold (backorder).
        public decimal Count { get; set; }
    }
}
