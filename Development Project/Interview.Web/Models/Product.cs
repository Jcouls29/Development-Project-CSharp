using System;
using System.Collections.Generic;

namespace Interview.Web.Models
{
    public class Product
    {
        public int InstanceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProductImageUris { get; set; }
        public string ValidSKUs { get; set; }
        public DateTime CreatedTimestamp { get; set; }

        public virtual ICollection<InventoryTransaction> InventoryTransactions { get; set; }
        public virtual ICollection<ProductAttribute> ProductAttributes { get; set; }
        public virtual ICollection<ProductCategory> ProductCategories { get; set; }
    }
}
