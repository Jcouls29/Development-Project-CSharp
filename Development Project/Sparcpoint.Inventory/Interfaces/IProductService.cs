using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Interfaces
{
    public interface IProductService
    {
        Task<Models.ProductDetailModel> CreateProductAsync(Models.CreateProductRequest request);
        Task<Models.ProductDetailModel> GetProductAsync(int instanceId);
        Task<Models.PaginatedResult<Models.ProductModel>> SearchProductsAsync(Models.ProductSearchRequest request);
    }
}
