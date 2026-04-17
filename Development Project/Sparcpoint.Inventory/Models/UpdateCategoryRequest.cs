using System.Collections.Generic;

namespace Sparcpoint.Inventory.Models
{
    public class UpdateCategoryRequest
    {
        public Dictionary<string, string> Attributes { get; set; }
        public List<int> ParentCategoryIds { get; set; }
    }
}
