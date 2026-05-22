using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sparcpoint.Inventory.Models.DTOs.Requests
{
    /// <summary>
    /// Request to remove inventory for one or more products.
    /// </summary>
    public class RemoveInventoryRequest
    {
        [Required]
        [MinLength(1, ErrorMessage = "At least one inventory item is required")]
        public List<InventoryItem> Items { get; set; } = new List<InventoryItem>();
    }
}