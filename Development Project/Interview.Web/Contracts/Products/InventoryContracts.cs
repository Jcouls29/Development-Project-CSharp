namespace Interview.Web.Contracts.Products
{
    public class AdjustInventoryRequest
    {
        public decimal Quantity { get; set; }
        public string TypeCategory { get; set; }
    }

    public class AdjustInventoryResponse
    {
        public int TransactionId { get; set; }
        public decimal CurrentQuantity { get; set; }
    }

    public class ProductInventoryCountResponse
    {
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
    }
}