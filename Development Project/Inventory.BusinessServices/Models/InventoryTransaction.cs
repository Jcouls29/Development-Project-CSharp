namespace Inventory.BusinessServices
{
    public class InventoryTransaction
    {
        // prefer Guids rather than id
        public int TransactionId { get; set; }
        public int ProductInstanceId { get; set; } // This is product id
        public int CategoryId { get; set; } // This should be id in database. Separate category table
        public int Quantity { get; set; }
    }
}