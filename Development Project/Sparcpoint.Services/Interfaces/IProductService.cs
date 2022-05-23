using Sparcpoint.Models.Request.Product;
using Sparcpoint.Models.Response.Product;
using System.Threading.Tasks;

namespace Sparcpoint.Services.Interfaces
{
    public interface IProductService
    {
        Task<GetProductResponse> GetAllProducts(GetProductRequest request);

        Task<ProductResponse> GetProductById(GetProductRequestById request);

        Task<InsertProductResponse> InsertProduct(InsertProductRequest insertProductRequest);

        Task<UpdateProductResponse> UpdateProduct(UpdateProductRequest updateProductRequest);
    }
}