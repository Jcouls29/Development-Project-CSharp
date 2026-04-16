using System.ComponentModel.DataAnnotations;

namespace Interview.DataEntities.Models
{
    public class CategoryRequest
    {
        [Required]
        [StringLength(64)]
        public string Name { get; set; }

        [Required]
        [StringLength(256)]
        public string Description { get; set; }

        public int? ParentCategoryId { get; set; }
    }
}
