using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Infrastructure.Persistence.Entities
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InstanceId { get; set; }

        [Required]
        [MaxLength(256)]
        public string Name { get; set; }

        [Required]
        [MaxLength(256)]
        public string Description { get; set; }

        [Required]
        public string ProductImageUris { get; set; }

        [Required]
        public string ValidSkus { get; set; }

        [Required]
        public DateTime CreatedTimestamp { get; set; } = DateTime.UtcNow;

        public virtual ICollection<ProductCategory> ProductCategories { get; set; }

        public virtual ICollection<ProductAttribute> ProductAttributes { get; set; }
    }
}
