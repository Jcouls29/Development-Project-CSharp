namespace Sparcpoint.Inventory.Models
{
    public class InventoryAdjustment
    {
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public string TypeCategory { get; set; }
    }
}
