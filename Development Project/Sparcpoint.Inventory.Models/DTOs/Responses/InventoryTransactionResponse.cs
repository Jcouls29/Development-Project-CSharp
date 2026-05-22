using System;

namespace Sparcpoint.Inventory.Models.DTOs.Responses
{
    public class InventoryTransactionResponse
    {
        public int TransactionId { get; set; }
        public int ProductInstanceId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public DateTime StartedTimestamp { get; set; }
        public DateTime? CompletedTimestamp { get; set; }
        public string? TypeCategory { get; set; }
    }
}