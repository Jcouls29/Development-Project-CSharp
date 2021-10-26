using System.Collections.Generic;

namespace Interview.Data.Models
{
    public partial class Product
    {
        public Product()
        {
            ProductAttributes = new HashSet<ProductAttribute>();
            Categories = new HashSet<Category>();
        }
    
        public int InstanceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProductImageUris { get; set; }
        public string ValidSkus { get; set; }
        public System.DateTime CreatedTimestamp { get; set; }
    
        public ICollection<ProductAttribute> ProductAttributes { get; set; }
        public ICollection<Category> Categories { get; set; }
    }
}
