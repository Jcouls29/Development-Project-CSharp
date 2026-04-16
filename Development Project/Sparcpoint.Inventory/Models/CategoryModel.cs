using System;

namespace Sparcpoint.Inventory.Models
{
    public class CategoryModel
    {
        public int InstanceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedTimestamp { get; set; }
    }
}
