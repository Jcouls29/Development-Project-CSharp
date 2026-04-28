namespace Sparcpoint.Inventory.Abstractions
{
    /// <summary>
    /// Aggregated inventory count for a single product.
    /// </summary>
    public class ProductInventoryCount
    {
        public int ProductInstanceId { get; set; }
        public string ProductName { get; set; }
        public decimal TotalQuantity { get; set; }
    }
}
