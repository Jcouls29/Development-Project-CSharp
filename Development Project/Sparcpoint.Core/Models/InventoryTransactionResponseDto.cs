namespace Sparcpoint.Core.Models
{
    public class InventoryTransactionResponseDto
    {
        public int TransactionId { get; set; }
        public int ProductInstanceId { get; set; }
        public decimal Quantity { get; set; }
    }
}
