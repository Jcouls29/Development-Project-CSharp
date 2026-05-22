using System;
using System.Collections.Generic;

namespace Sparcpoint.Inventory.Models
{
    public class CreateCategoryRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IReadOnlyList<int> ParentCategoryIds { get; set; } = Array.Empty<int>();
        public IReadOnlyDictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
    }
}
