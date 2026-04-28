namespace Sparcpoint.Inventory.Abstractions
{
    /// <summary>
    /// A single item in a bulk inventory operation.
    /// </summary>
    public class InventoryBatchItem
    {
        public int ProductInstanceId { get; set; }
        public decimal Quantity { get; set; }
        public string TypeCategory { get; set; }
    }
}
