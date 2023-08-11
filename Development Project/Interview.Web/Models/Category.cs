using System;
using System.Collections.Generic;

namespace Interview.Web.Models
{
    public partial class Category
    {
        public int InstanceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedTimestamp { get; set; }

        public virtual ICollection<CategoryAttribute> CategoryAttributes { get; set; }
        public virtual ICollection<CategoryCategory> CategoryCategoriesCategoryInstances { get; set; }
        public virtual ICollection<CategoryCategory> CategoryCategoriesInstances { get; set; }
        public virtual ICollection<ProductCategory> ProductCategories { get; set; }
    }
}
