using System;
using System.Collections.Generic;

namespace Sparcpoint.Models
{
    public class Product
    {
        public int InstanceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> ProductImageUris { get; set; } = new List<string>();
        public List<string> ValidSkus { get; set; } = new List<string>();
        public DateTime CreatedTimestamp { get; set; }
    }
}
