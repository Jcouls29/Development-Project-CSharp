using System;
using System.Collections.Generic;

namespace Interview.DataEntities.Models
{
    public class ProductResponse
    {
        public int InstanceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProductImageUris { get; set; }
        public string ValidSkus { get; set; }
        public DateTime CreatedTimestamp { get; set; }
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
        public List<string> Categories { get; set; } = new List<string>();
    }
}
