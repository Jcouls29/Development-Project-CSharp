using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ProductAttribute
    {
        [Key]
        public int InstanceId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public int ProductInstanceId { get; set; }
    }
}
