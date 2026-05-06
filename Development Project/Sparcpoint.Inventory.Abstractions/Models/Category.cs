using System;
using System.Collections.Generic;

namespace Sparcpoint.Inventory.Abstractions
{
    /// <summary>
    /// Represents a product category. Categories support hierarchy via
    /// Instances.CategoryCategories (adjacency-list pattern).
    /// </summary>
    public class Category
    {
        public int InstanceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedTimestamp { get; set; }
        public List<int> ParentCategoryIds { get; set; } = new List<int>();
    }
}
