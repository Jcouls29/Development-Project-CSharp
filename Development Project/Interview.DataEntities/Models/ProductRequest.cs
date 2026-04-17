using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Interview.DataEntities.Models
{
    public class ProductRequest
    {
        [Required]
        [StringLength(256)]
        public string Name { get; set; }
        [Required]
        [StringLength(256)]
        public string Description { get; set; }
        [Required]
        public string ProductImageUris { get; set; }
        [Required]
        public string ValidSkus { get; set; }
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
    }
}
