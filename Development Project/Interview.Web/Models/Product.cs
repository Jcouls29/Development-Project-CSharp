using System;
using System.Collections.Generic;

namespace Interview.Web.Models
{
    public partial class Product
    {
        public Product()
        {
            ProductAttributes = new HashSet<ProductAttribute>();
            CategoryInstances = new HashSet<Category>();
        }

        public int InstanceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProductImageUris { get; set; }
        public string ValidSkus { get; set; }
        public DateTime CreatedTimestamp { get; set; }

        public virtual ICollection<ProductAttribute> ProductAttributes { get; set; }

        public virtual ICollection<Category> CategoryInstances { get; set; }
    }
}
