using System;

namespace Interview.Web.Models
{
    public class InventoryTransactionCreateDto
    {
        public Guid ProductId { get; set; }
        public decimal Quantity { get; set; }
        public InventoryTransactionType Type { get; set; }
    }
}
