using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Interfaces
{
    public interface IProductRepository
    {
        Task<int> CreateAsync(Models.CreateProductRequest request);
        Task<Models.ProductDetailModel> GetByIdAsync(int instanceId);
        Task<Models.PaginatedResult<Models.ProductModel>> SearchAsync(Models.ProductSearchRequest request);
    }
}
