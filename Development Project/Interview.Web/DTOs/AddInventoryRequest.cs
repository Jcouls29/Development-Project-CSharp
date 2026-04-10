using System.ComponentModel.DataAnnotations;

namespace Interview.Web.DTOs
{
    /// <summary>
    /// EVAL: Request DTO for adding or removing inventory.
    /// Quantity must be positive — the API endpoint determines the direction (add/remove).
    /// </summary>
    public class InventoryTransactionRequest
    {
        [Required]
        [Range(0.000001, double.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
        public decimal Quantity { get; set; }

        public string? TypeCategory { get; set; }
    }
}
