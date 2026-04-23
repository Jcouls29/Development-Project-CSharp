using System;

namespace Interview.Web.Models
{
    public class AddInventoryRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class InventoryCountResponse
    {
        public int ProductId { get; set; }
        public int TotalQuantity { get; set; }
    }

    public class InventoryTransactionDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public string TransactionType => Quantity > 0 ? "ADD" : "REMOVE";
    }

    public class RemoveInventoryRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
