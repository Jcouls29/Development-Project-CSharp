using System.Collections.Generic;

namespace Sparcpoint.Inventory.Models
{
    public class InventoryCountByMetadataModel
    {
        public List<InventoryCountModel> Items { get; set; }
        public decimal TotalQuantity { get; set; }
    }
}
