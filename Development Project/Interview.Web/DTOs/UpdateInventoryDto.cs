using System.ComponentModel.DataAnnotations;

namespace Interview.Web.DTOs
{
    public class UpdateInventoryDto
    {
        [Required(ErrorMessage = "ProductId required.")]
        [Range(1, int.MaxValue, ErrorMessage = "ProductId must be a valid.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Quantity required.")]
        public decimal Quantity { get; set; }

        [StringLength(32, ErrorMessage = "The TypeCategory cannot exceed 32 characters.")]
        public string TypeCategory { get; set; }
    }
}
