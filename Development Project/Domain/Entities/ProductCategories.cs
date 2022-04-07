using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    
    public class ProductCategory
    {
        [Key]
        public int InstanceId { get; set; }
        public int CategoryInstanceId { get; set; }
        public int ProductInstanceId { get; set; }
    }
}
