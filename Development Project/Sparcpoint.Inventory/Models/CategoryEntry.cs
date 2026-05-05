using System;
using System.Collections.Generic;

namespace Sparcpoint.Inventory.Models
{
    public class CategoryEntry
    {
        public int InstanceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedTimestamp { get; set; }
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
        public int[] ParentCategoryIds { get; set; } = Array.Empty<int>();
    }
}
