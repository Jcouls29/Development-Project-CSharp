namespace Sparcpoint.Inventory.Models.Requests
{
    public class InventoryAdjustmentRequest
    {
        public int ProductInstanceId { get; set; }
        public decimal Quantity { get; set; }

        // EVAL: TypeCategory maps to the existing VARCHAR(32) NULL column in InventoryTransactions;
        // its intended use is not defined in the schema — passed through as optional to avoid data loss
        public string TypeCategory { get; set; }
    }
}
