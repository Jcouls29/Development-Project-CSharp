using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sparcpoint.Inventory.Models
{
    public class CreateProductRequest
    {
        [Required]
        [MaxLength(256)]
        public string Name { get; set; }

        [MaxLength(256)]
        public string Description { get; set; }

        public List<string> ProductImageUris { get; set; }
        public List<string> ValidSkus { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public List<int> CategoryIds { get; set; }
    }
}
