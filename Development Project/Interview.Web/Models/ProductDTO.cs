using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Interview.Web.Models
{
    [Table("Products")]
    public class ProductDTO
    {
        [Key]
        public int InstanceId { get; set; }
        public string Name { get; set; }    
        public string Description { get; set; } 
        public string ProductImageUris { get; set; }
        public string ValidSkus { get; set; }
        public DateTime CreatedTimestamp { get; set; }
    }
}
