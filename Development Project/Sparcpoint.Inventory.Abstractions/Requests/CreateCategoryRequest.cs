using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sparcpoint.Inventory.Abstractions
{
    public class CreateCategoryRequest
    {
        [Required]
        [MaxLength(64)]
        public string Name { get; set; }

        [Required]
        [MaxLength(256)]
        public string Description { get; set; }

        /// <summary>
        /// Parent category IDs — supports hierarchical categorization.
        /// Stored in Instances.CategoryCategories.
        /// </summary>
        public IEnumerable<int> ParentCategoryIds { get; set; } = new List<int>();
        public IDictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
    }
}
