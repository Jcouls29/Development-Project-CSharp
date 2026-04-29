using System;
using System.Collections.Generic;
using System.Text;

namespace Sparcpoint.Models.Entity
{
    public class Product
    {
        public int InstanceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProductImageUris { get; set; }
        public string ValidSkus { get; set; } 

        public DateTime CreatedTimestamp { get; set; }

        public IList<ProductAttribute> Metadata { get; set; } = new List<ProductAttribute>();

        public IList<int> CategoryIds { get; set; } = new List<int>();
    }
}
