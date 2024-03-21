using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Infrastructure.Persistence.Entities
{
    public class CategoryAttribute
    {
        [Key]
        [Column(Order = 1)]
        public int InstanceId { get; set; }

        [Key]
        [MaxLength(64)]
        [Column(Order = 2)]
        public string Key { get; set; }

        [Required]
        [MaxLength(512)]
        public string Value { get; set; }

        [ForeignKey("InstanceId")]
        public virtual Category Category { get; set; }
    }
}

