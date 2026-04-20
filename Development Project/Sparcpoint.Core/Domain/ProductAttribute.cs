using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Sparcpoint.Domain
{
    [Table("ProductAttributes", Schema = "Instances")]
    public class ProductAttribute
    {
        
        public int InstanceId { get; set; }

        public string Key { get; set; }
        public string Value { get; set; }
    }
}
