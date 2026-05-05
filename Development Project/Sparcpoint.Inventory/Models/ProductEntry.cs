using System;
using System.Collections.Generic;

namespace Sparcpoint.Inventory.Models
{
    public class ProductEntry
    {
        public int InstanceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string[] ProductImageUris { get; set; } = Array.Empty<string>();
        public string[] ValidSkus { get; set; } = Array.Empty<string>();
        public DateTime CreatedTimestamp { get; set; }
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
        public int[] CategoryIds { get; set; } = Array.Empty<int>();
    }
}
