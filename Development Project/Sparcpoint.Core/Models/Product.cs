using System;
using System.Collections.Generic;
using Sparcpoint.Models;

namespace Sparcpoint.Core.Models
{
    public class Product
    {
        public int InstanceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public IList<string> ProductImageUris { get; set; }
        public IList<string> ValidSkus { get; set; }
        public DateTime CreatedTimestamp { get; set; }

        public IEnumerable<ProductCategory> Categories { get; set; }
        public IEnumerable<ProductMetadata> Metadata { get; set; }
    }
}
