namespace Sparcpoint.Models.Request.Product
{
    public class GetProductRequestById
    {
        public int ProductId { get; set; }

        public GetProductRequestById(int productId)
        {
            this.ProductId = productId;
        }
    }
}