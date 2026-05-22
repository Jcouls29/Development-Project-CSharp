using System;
using System.Collections.Generic;

namespace Sparcpoint.Inventory.Models
{
    public class ProductSearchCriteria
    {
        public string NameContains { get; set; }
        public IReadOnlyList<int> CategoryIds { get; set; } = Array.Empty<int>();
        public IReadOnlyDictionary<string, string> AttributeFilters { get; set; } = new Dictionary<string, string>();
    }
}
