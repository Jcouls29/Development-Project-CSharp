namespace Sparcpoint.Inventory.Models
{
    /// <summary>
    /// EVAL: Result grouped by product. Allows returning a single product
    /// or multiple when the query is by metadata/category.
    /// </summary>
    public sealed class InventoryCountResult
    {
        public int ProductInstanceId { get; set; }
        public string ProductName { get; set; }
        public decimal Count { get; set; }
    }
}
