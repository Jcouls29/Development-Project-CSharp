using System;
using System.Collections.Generic;

namespace Sparcpoint.Inventory.Models
{
    public sealed class Category
    {
        public int InstanceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedTimestamp { get; set; }

        public IReadOnlyList<ProductAttribute> Attributes { get; set; } = Array.Empty<ProductAttribute>();
        public IReadOnlyList<int> ParentCategoryIds { get; set; } = Array.Empty<int>();
    }
}
