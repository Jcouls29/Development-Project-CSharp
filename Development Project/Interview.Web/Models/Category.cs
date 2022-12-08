using System;
using System.Collections.Generic;

#nullable disable

namespace Interview.Web.Models
{
    public partial class Category
    {
        public Category()
        {
            CategoryAttributes = new HashSet<CategoryAttribute>();
            CategoryCategoryCategoryInstances = new HashSet<CategoryCategory>();
            CategoryCategoryInstances = new HashSet<CategoryCategory>();
            ProductCategories = new HashSet<ProductCategory>();
        }

        public int InstanceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedTimestamp { get; set; }

        public virtual ICollection<CategoryAttribute> CategoryAttributes { get; set; }
        //JsonIgnore prevents circular references when deserializing
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual ICollection<CategoryCategory> CategoryCategoryCategoryInstances { get; set; }
        public virtual ICollection<CategoryCategory> CategoryCategoryInstances { get; set; }
        public virtual ICollection<ProductCategory> ProductCategories { get; set; }
    }
}
