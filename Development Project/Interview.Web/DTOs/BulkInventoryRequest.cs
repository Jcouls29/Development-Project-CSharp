using System.ComponentModel.DataAnnotations;

namespace Interview.Web.DTOs
{
    /// <summary>
    /// EVAL: V2 DTO for bulk inventory operations.
    /// Includes ProductInstanceId so multiple products can be processed in one request.
    /// </summary>
    public class BulkInventoryRequest
    {
        [Required]
        public int ProductInstanceId { get; set; }

        [Required]
        [Range(0.000001, double.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
        public decimal Quantity { get; set; }

        public string TypeCategory { get; set; }
    }
}
