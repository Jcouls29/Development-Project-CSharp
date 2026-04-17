namespace Sparcpoint.Core.Models
{
    public class InventoryAdjustDto
    {
        // Product instance id in the Instances.Products table
        public int ProductId { get; set; }
        // Quantity uses decimal(19,6) in the DB
        public decimal Quantity { get; set; }
        public string TransactionType { get; set; }
    }
}
