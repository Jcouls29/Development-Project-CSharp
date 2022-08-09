using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Sparcpoint.Models
{
    public class Category
    {
        [Key]
        public Guid InstanceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public virtual ICollection<CatergoryAttributes> Attributes { get; set; }
        public virtual ICollection<CategoryOfCategory> Categories { get; set; }
    }
}
