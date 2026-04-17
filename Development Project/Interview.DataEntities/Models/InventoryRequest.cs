using System.ComponentModel.DataAnnotations;

namespace Interview.DataEntities.Models
{
    public class InventoryRequest
    {
        [Required]
        public int ProductInstanceId { get; set; }
        [Required]
        public decimal Quantity { get; set; }
        [StringLength(32)]
        public string TypeCategory { get; set; }
    }
}
