using System.ComponentModel.DataAnnotations;

namespace Sparcpoint.Inventory.Models
{
    public class InventoryTransactionRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "ProductInstanceId must be greater than 0.")]
        public int ProductInstanceId { get; set; }

        [Required]
        public decimal Quantity { get; set; }

        public string TypeCategory { get; set; }
    }
}
