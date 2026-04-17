using System;
using System.Collections.Generic;

namespace Sparcpoint.Inventory.Models
{
    public class ProductModel
    {
        public int InstanceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> ProductImageUris { get; set; }
        public List<string> ValidSkus { get; set; }
        public DateTime CreatedTimestamp { get; set; }
    }
}
