using System;
using System.Collections.Generic;

namespace Interview.Web.Models
{
    public partial class Category
    {
        public Category()
        {
            CategoryAttributes = new HashSet<CategoryAttribute>();
            CategoryInstances = new HashSet<Category>();
            Instances = new HashSet<Category>();
            InstancesNavigation = new HashSet<Product>();
        }

        public int InstanceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedTimestamp { get; set; }

        public virtual ICollection<CategoryAttribute> CategoryAttributes { get; set; }

        public virtual ICollection<Category> CategoryInstances { get; set; }
        public virtual ICollection<Category> Instances { get; set; }
        public virtual ICollection<Product> InstancesNavigation { get; set; }
    }
}
