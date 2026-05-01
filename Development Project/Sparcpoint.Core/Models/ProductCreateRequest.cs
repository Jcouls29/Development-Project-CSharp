using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sparcpoint.Core.Models
{   
    public class ProductCreateRequest
    {
        [Required]
        [StringLength(256)]
        public string Name { get; set; }

        [StringLength(256)]
        public string Description { get; set; }

        public IEnumerable<string> ProductImageUris { get; set; }

        public IEnumerable<string> ValidSkus { get; set; }

        // Metadata keys/values validated in service/validator
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();

        public IEnumerable<int> CategoryIds { get; set; }
    }
}