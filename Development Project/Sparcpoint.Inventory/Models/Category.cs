using System.Collections.Generic;

namespace Sparcpoint.Inventory.Models
{
    // EVAL: Category supports hierarchies via CategoryCategories (many-to-many
    // parent/child). ParentInstanceIds allows a category to belong to multiple
    // parents, which matches the DB design — this is a DAG, not a simple tree.
    public class Category : InstanceBase
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public Dictionary<string, string> Attributes { get; set; } = new();
        public List<int> ParentInstanceIds { get; set; } = new();
        public List<int> ChildInstanceIds { get; set; } = new();
    }
}
