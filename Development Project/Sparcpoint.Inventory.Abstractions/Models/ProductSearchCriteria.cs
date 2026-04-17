using System.Collections.Generic;

namespace Sparcpoint.Inventory.Models
{
    /// <summary>
    /// EVAL: Combinable search criteria (all optional). Intentionally
    /// "additive": adding a new criterion (e.g. PriceRange) does not break previous
    /// clients (open/closed principle).
    /// </summary>
    public sealed class ProductSearchCriteria
    {
        /// <summary>Partial text against Product.Name (LIKE %text%).</summary>
        public string NameContains { get; set; }

        /// <summary>Filters by attribute key/value (exact match). Multiple entries are AND.</summary>
        public IList<ProductAttribute> AttributeFilters { get; set; } = new List<ProductAttribute>();

        /// <summary>Category IDs: the product must belong to ALL of the indicated ones (AND).</summary>
        public IList<int> CategoryIds { get; set; } = new List<int>();

        /// <summary>Pagination: offset and limit. Avoids unbounded queries.</summary>
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 100;
    }
}
