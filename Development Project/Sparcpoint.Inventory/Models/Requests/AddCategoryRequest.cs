using System.Collections.Generic;

namespace Sparcpoint.Inventory.Models.Requests
{
    public class AddCategoryRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }

        // EVAL: CategoryCategories is a many-to-many hierarchy table, so a category can have
        // multiple parents. Optional — omit for a top-level category.
        public IEnumerable<int> ParentCategoryIds { get; set; } = new List<int>();
    }
}
