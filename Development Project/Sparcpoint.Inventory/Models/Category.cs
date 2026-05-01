using System;
using System.Collections.Generic;

namespace Sparcpoint.Inventory.Models
{
    public class Category
    {
        public int InstanceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedTimestamp { get; set; }
        public IEnumerable<int> ParentCategoryIds { get; set; } = new List<int>();
    }
}
