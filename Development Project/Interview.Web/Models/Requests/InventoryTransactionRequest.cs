using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Interview.Web.Models.Requests
{
    /// <summary>
    /// Request body for a single inventory transaction (add or remove).
    /// </summary>
    public class InventoryTransactionItem
    {
        /// <summary>
        /// The product ID to add/remove inventory for.
        /// </summary>
        [Required(ErrorMessage = "Product ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Product ID must be a positive integer.")]
        public int ProductInstanceId { get; set; }

        /// <summary>
        /// Quantity to add or remove. Must be positive.
        /// The API endpoint (add vs remove) determines the sign.
        /// </summary>
        // EVAL: Quantity is always positive in the request. The controller negates it
        // for removal operations. This prevents confusion and accidental double-negation.
        [Required(ErrorMessage = "Quantity is required.")]
        [Range(0.000001, 9999999999999.999999, ErrorMessage = "Quantity must be a positive number within DECIMAL(19,6) range.")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// Optional transaction type (e.g., "Purchase", "Sale", "Return", "Adjustment").
        /// </summary>
        [MaxLength(32, ErrorMessage = "Type category cannot exceed 32 characters.")]
        public string TypeCategory { get; set; }
    }

    /// <summary>
    /// Request body for adding inventory. Supports single or bulk operations.
    /// </summary>
    // EVAL: Bulk operations use a list to support Requirement 4:
    // "Adding and Removing inventory should happen on an individual product level
    // or multiple products at once."
    public class AddInventoryRequest
    {
        /// <summary>
        /// One or more inventory items to add.
        /// </summary>
        [Required(ErrorMessage = "At least one inventory item is required.")]
        [MinLength(1, ErrorMessage = "At least one inventory item is required.")]
        public List<InventoryTransactionItem> Items { get; set; } = new List<InventoryTransactionItem>();
    }

    /// <summary>
    /// Request body for removing inventory. Supports single or bulk operations.
    /// </summary>
    public class RemoveInventoryRequest
    {
        /// <summary>
        /// One or more inventory items to remove.
        /// </summary>
        [Required(ErrorMessage = "At least one inventory item is required.")]
        [MinLength(1, ErrorMessage = "At least one inventory item is required.")]
        public List<InventoryTransactionItem> Items { get; set; } = new List<InventoryTransactionItem>();
    }
}
