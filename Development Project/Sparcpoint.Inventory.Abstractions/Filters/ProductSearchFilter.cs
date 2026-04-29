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
    }
}
