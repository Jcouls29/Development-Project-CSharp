using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Sparcpoint.Domain
{
    [Table("Products", Schema = "Instances")]
    public class Product
    {
        [Key]
        public int InstanceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProductImageUris { get; set; }
        public string ValidSkus { get; set; }
        public DateTime CreatedTimestamp { get; set; } = DateTime.UtcNow;

        [ForeignKey("InstanceId")]
        public virtual List<ProductAttribute> Attributes { get; private set; } = new List<ProductAttribute>();

        public virtual List<Category> Categories { get; set; } = new List<Category>();

        public void SetAttribute(string name, string value)
        {
            name = name.ToLower();
            var attribute = Attributes.Find(a => a.Key == name);
            if (attribute == null)
            {
                attribute = new ProductAttribute { Key = name, Value = value };
                Attributes.Add(attribute);
            }
            else
            {
                attribute.Value = value;
            }
        }

        public void SetCategory(int categoryId)
        {
            var category = Categories.Find(c => c.InstanceId == categoryId);
            if (category == null)
            {
                category = new Category { InstanceId = categoryId };
                Categories.Add(category);
            }
        }
    }
}
