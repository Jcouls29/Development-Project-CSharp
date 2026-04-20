using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sparcpoint.Domain
{
    [Table("Categories", Schema = "Instances")]
    public class Category
    {
        [Key]
        public int InstanceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public DateTime CreatedTimestamp { get; set; }

        public virtual List<Product> Products { get; private set; }
    }
}
