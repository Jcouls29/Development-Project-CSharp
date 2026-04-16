namespace Interview.Web.Models
{
    public class InventoryTransactionRequest
    {
        public int ProductInstanceId { get; set; }
        public decimal Quantity { get; set; }
        public string TypeCategory { get; set; }
    }

    public class InventoryResponse
    {
        public int ProductInstanceId { get; set; }
        public decimal CurrentStock { get; set; }
    }
}
