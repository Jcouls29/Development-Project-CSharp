using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sparcpoint.Inventory.Models.DTOs.Requests
{
    /// <summary>
    /// Request to add inventory for one or more products.
    /// EVAL: Supports bulk operations as per requirement.
    /// </summary>
    public class AddInventoryRequest
    {
        [Required]
        [MinLength(1, ErrorMessage = "At least one inventory item is required")]
        public List<InventoryItem> Items { get; set; } = new List<InventoryItem>();
    }

    public class InventoryItem
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "ProductInstanceId must be greater than 0")]
        public int ProductInstanceId { get; set; }
        
        [Required]
        [Range(0.000001, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public decimal Quantity { get; set; }
        
        /// <summary>
        /// Optional transaction type category (e.g., "PURCHASE", "RETURN", "ADJUSTMENT").
        /// </summary>
        [StringLength(32)]
        public string? TypeCategory { get; set; }
    }
}