using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sparcpoint.Inventory.Models
{
    public class CreateCategoryRequest
    {
        [Required]
        [MaxLength(64)]
        public string Name { get; set; }

        [MaxLength(256)]
        public string Description { get; set; }

        public Dictionary<string, string> Attributes { get; set; }
        public List<int> ParentCategoryIds { get; set; }
    }
}
