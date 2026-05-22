using System;
using System.Collections.Generic;

namespace Sparcpoint.Inventory.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public IReadOnlyList<int> ParentCategoryIds { get; set; } = Array.Empty<int>();
        public IReadOnlyDictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
        public DateTime CreatedTimestamp { get; set; }
    }
}
