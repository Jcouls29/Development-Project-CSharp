namespace Sparcpoint.Core.Models
{
    // EVAL: Domain model for bulk inventory operations
    public class InventoryAdjustment
    {
        public int ProductInstanceId { get; set; }
        public decimal Quantity { get; set; }
        public string TypeCategory { get; set; }
    }
}
