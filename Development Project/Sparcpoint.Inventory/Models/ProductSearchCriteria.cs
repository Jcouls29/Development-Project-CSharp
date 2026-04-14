using System;
using System.Collections.Generic;

namespace Sparcpoint.Inventory.Models
{
    /// EVAL: Open-closed — new filters can be added here without
    /// breaking existing callers (all properties optional).
    public sealed class ProductSearchCriteria
    {
        public string? NameContains { get; set; }
        public string? DescriptionContains { get; set; }
        public IReadOnlyList<int> CategoryIds { get; set; } = Array.Empty<int>();
        public IReadOnlyList<ProductAttribute> AttributeMatches { get; set; } = Array.Empty<ProductAttribute>();
        public string? Sku { get; set; }

        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 50;
    }
}
