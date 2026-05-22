namespace Sparcpoint.Inventory.Models.DTOs.Responses
{
    /// <summary>
    /// Response for inventory count queries.
    /// </summary>
    public class InventoryCountResponse
    {
        public int? ProductInstanceId { get; set; }
        public string? ProductName { get; set; }
        public decimal TotalQuantity { get; set; }
        public int TransactionCount { get; set; }
    }
}