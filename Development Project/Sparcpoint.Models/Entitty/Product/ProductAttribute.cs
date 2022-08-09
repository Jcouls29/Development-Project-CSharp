using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Sparcpoint.Models
{
    public class ProductAttribute
    {
        [ForeignKey("Product")]
        public Guid InstanceId { get; set; } 
        public string Key { get; set; }
        public string Value { get; set; }
        private Products Product { get; set; }
    }
}
