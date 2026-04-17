namespace Sparcpoint.Inventory.Models
{
    /// <summary>
    /// EVAL: Unit of work for bulk add/remove. Quantity is signed:
    /// positive = add, negative = remove. TypeCategory is optional.
    /// </summary>
    public sealed class InventoryAdjustment
    {
        public int ProductInstanceId { get; set; }
        public decimal Quantity { get; set; }
        public string TypeCategory { get; set; }

        public InventoryAdjustment() { }

        public InventoryAdjustment(int productInstanceId, decimal quantity, string typeCategory = null)
        {
            ProductInstanceId = productInstanceId;
            Quantity = quantity;
            TypeCategory = typeCategory;
        }
    }
}
