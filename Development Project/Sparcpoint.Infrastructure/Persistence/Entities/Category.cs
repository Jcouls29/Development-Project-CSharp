using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Infrastructure.Persistence.Entities
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InstanceId { get; set; }

        [Required]
        [MaxLength(64)]
        public string Name { get; set; }

        [Required]
        [MaxLength(256)]
        public string Description { get; set; }

        [Required]
        public DateTime CreatedTimestamp { get; set; } = DateTime.UtcNow;

        public virtual ICollection<ProductCategory> ProductCategories { get; set; }

        public virtual ICollection<CategoryCategory> ParentCategories { get; set; }

        public virtual ICollection<CategoryCategory> ChildCategories { get; set; }

        public virtual ICollection<CategoryAttribute> CategoryAttributes { get; set; }
    }
}
