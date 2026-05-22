using System.Collections.Generic;

namespace Sparcpoint.Inventory.Models
{
    public class InventoryCountQuery
    {
        public int? ProductId { get; set; }
        public IReadOnlyDictionary<string, string> AttributeFilters { get; set; } = new Dictionary<string, string>();
    }
}
