namespace Sparcpoint.Inventory.Models
{
    /// <summary>
    /// EVAL: Search criteria supporting flexible product queries.
    /// All fields are optional — when none are provided, all products are returned.
    /// Multiple criteria are combined with AND logic for precise filtering.
    /// </summary>
    public class ProductSearchCriteria
    {
        public string? NameContains { get; set; }
        public string? DescriptionContains { get; set; }
        public List<int>? CategoryIds { get; set; }
        public Dictionary<string, string>? Attributes { get; set; }
        public string? SkuContains { get; set; }
    }
}
