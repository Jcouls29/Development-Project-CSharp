using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Sparcpoint.Models
{
    public class Products
    {
        [Key]
        public Guid InstanceId { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProductImageUri { get; set; }
        public string ValidSkus { get; set; }
        public DateTime CreatedDate { get; set; }
        public virtual ICollection<ProductAttribute> Attributes { get; set; }
        public virtual ICollection<ProductCategories> Categories { get; set; }

    }
}
