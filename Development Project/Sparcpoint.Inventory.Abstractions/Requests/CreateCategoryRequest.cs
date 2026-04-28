using System.Collections.Generic;

namespace Sparcpoint.Inventory.Abstractions
{
    public class CreateCategoryRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// Parent category IDs — supports hierarchical categorization.
        /// Stored in Instances.CategoryCategories.
        /// </summary>
        public IEnumerable<int> ParentCategoryIds { get; set; } = new List<int>();
        public IDictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
    }
}
