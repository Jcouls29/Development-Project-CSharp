using System;

namespace Interview.Web.Models
{
    public class InventoryTransactionResponseDto
    {
        public Guid TransactionId { get; set; }
        public Guid ProductId { get; set; }
        public decimal Quantity { get; set; }
        public InventoryTransactionType Type { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
