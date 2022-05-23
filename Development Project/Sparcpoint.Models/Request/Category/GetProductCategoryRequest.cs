namespace Sparcpoint.Models.Request.Category
{
    public class GetProductCategoryRequest
    {
        public int ProductId { get; set; }

        public GetProductCategoryRequest(int productId)
        {
            ProductId = productId;
        }
    }
}