namespace Sparcpoint.Inventory.Models
{
    /// <summary>
    /// EVAL: Category entity maps to [Instances].[Categories] table.
    /// Supports hierarchical relationships via [Instances].[CategoryCategories].
    /// </summary>
    public class Category
    {
        public int InstanceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedTimestamp { get; set; }
        public Dictionary<string, string> Attributes { get; set; } = new();
        public List<int> ParentCategoryIds { get; set; } = new();
    }
}
