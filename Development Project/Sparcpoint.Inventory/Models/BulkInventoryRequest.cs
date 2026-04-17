using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sparcpoint.Inventory.Models
{
    public class BulkInventoryRequest
    {
        [Required]
        [MaxLength(100, ErrorMessage = "Bulk requests are limited to 100 items.")]
        public List<InventoryTransactionRequest> Items { get; set; }
    }
}
