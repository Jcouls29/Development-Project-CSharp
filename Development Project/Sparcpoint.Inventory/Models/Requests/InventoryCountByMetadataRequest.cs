using System.Collections.Generic;

namespace Sparcpoint.Inventory.Models.Requests
{
    public class InventoryCountByMetadataRequest
    {
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
    }
}
