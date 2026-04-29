using System.Collections.Generic;

namespace Sparcpoint.Inventory.Abstractions
{
    /// <summary>
    /// All fields are optional. Only non-null/non-empty fields are applied as filters.
    /// Multiple attribute entries are combined with AND semantics.
    /// </summary>
    public class ProductSearchFilter
    {
        public string Name { get; set; }
        public IDictionary<string, string> Attributes { get; set; }
        public int[] CategoryIds { get; set; }

        /// <summary>
        /// 1-based page number. Null disables pagination. Must be >= 1 when supplied.
        /// Both Page and PageSize must be non-null for pagination to activate.
        /// </summary>
        public int? Page { get; set; }

        /// <summary>
        /// Number of rows per page. Null disables pagination. Clamped to a maximum of 200.
        /// Both Page and PageSize must be non-null for pagination to activate.
        /// </summary>
        public int? PageSize { get; set; }
    }
}
