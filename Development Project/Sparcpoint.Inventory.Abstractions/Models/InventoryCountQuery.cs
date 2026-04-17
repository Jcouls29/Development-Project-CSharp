using System.Collections.Generic;

namespace Sparcpoint.Inventory.Models
{
    /// <summary>
    /// EVAL: Query to obtain counts. Can request:
    ///  - A specific product (ProductInstanceId).
    ///  - Multiple products by a subset of metadata (AttributeFilters).
    ///  - Include or exclude reverted transactions (IncludeReverted).
    /// </summary>
    public sealed class InventoryCountQuery
    {
        public int? ProductInstanceId { get; set; }
        public IList<ProductAttribute> AttributeFilters { get; set; } = new List<ProductAttribute>();
        public IList<int> CategoryIds { get; set; } = new List<int>();

        /// <summary>If false, only counts NON-reverted transactions (CompletedTimestamp IS NOT NULL).</summary>
        public bool IncludeReverted { get; set; } = false;
    }
}
