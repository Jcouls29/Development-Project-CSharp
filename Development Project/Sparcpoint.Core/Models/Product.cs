using System;
using System.Collections.Generic;

namespace Sparcpoint
{
    public class Product
    {
        public int InstanceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProductImageUris { get; set; }
        public string ValidSkus { get; set; }
        public DateTime CreatedTimestamp { get; set; }
        public List<int> CategoryInstanceIds { get; set; } = new List<int>();
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
    }
}