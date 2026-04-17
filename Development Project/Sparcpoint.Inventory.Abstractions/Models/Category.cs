using System;
using System.Collections.Generic;

namespace Sparcpoint.Inventory.Models
{
    /// <summary>
    /// EVAL: Category with optional hierarchy via <see cref="ParentCategoryIds"/>
    /// (DAG-modeled allowing multiple parents — flexible across clients).
    /// Mirrors [Instances].[Categories] + [Instances].[CategoryCategories].
    /// </summary>
    public sealed class Category
    {
        public int InstanceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedTimestamp { get; set; }

        public IList<CategoryAttribute> Attributes { get; set; } = new List<CategoryAttribute>();

        /// <summary>
        /// EVAL: Parent category IDs. Supports hierarchies (tree) and DAGs (multiple parents).
        /// </summary>
        public IList<int> ParentCategoryIds { get; set; } = new List<int>();
    }
}
