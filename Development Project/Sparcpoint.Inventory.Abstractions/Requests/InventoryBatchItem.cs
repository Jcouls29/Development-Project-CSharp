using System.ComponentModel.DataAnnotations;

namespace Sparcpoint.Inventory.Abstractions
{
    /// <summary>
    /// A single item in a bulk inventory operation.
    /// </summary>
    public class InventoryBatchItem
    {
        public int ProductInstanceId { get; set; }

        [Range(0.001, double.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
        public decimal Quantity { get; set; }
        public string TypeCategory { get; set; }
    }
}
