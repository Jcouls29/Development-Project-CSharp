using System.Collections.Generic;

namespace Sparcpoint.Inventory.Models
{
    public class BulkInventoryResult
    {
        public List<InventoryTransactionModel> Results { get; set; }
        public List<string> Errors { get; set; }
    }
}
