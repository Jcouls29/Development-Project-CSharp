using System.Collections.Generic;

namespace Interview.Data.Models
{
    public partial class Category
    {
        public Category()
        {
            CategoryAttributes = new HashSet<CategoryAttribute>();
            Categories1 = new HashSet<Category>();
            Categories = new HashSet<Category>();
            Products = new HashSet<Product>();
        }
    
        public int InstanceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public System.DateTime CreatedTimestamp { get; set; }
    
        public  ICollection<CategoryAttribute> CategoryAttributes { get; set; }
        public  ICollection<Category> Categories1 { get; set; }
        public  ICollection<Category> Categories { get; set; }
        public  ICollection<Product> Products { get; set; }
    }
}
