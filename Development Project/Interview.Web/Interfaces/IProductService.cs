using Interview.Web.DTO;
using Sparkpoint.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interview.Web.Interfaces
{
    public interface IProductService
    {
        IEnumerable<Product> GetAllProducts();
        Product GetProductById(int id);
        public Task<ProductCreationResponseDto> CreateProductAsync(ProductCreationRequestDto product);
        Product UpdateProduct(Product productId);
        Product DeleteProduct(int productId);
        public Task<bool> SoftDeleteProduct(int productId);
        IEnumerable<Product> SearchProducts(SearchCriteriaDto criteria);
        int GetInventoryCountByMetadata(SearchCriteriaDto criteria);
    }
}
