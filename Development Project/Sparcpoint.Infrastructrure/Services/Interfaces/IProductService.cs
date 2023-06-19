using Sparcpoint.Core.Persistence.Entity.Sparcpoint.Entities;

namespace Sparcpoint.Infrastructure.Services.Interfaces
{
    public interface IProductService
    {
        // EVAL: To handle many different companies this can be updated to include the companies identifier as a parameter
        // to only allow these functions to be called by the company that owns the product.
        // This could also be handled by a middleware that checks the company identifier on the request so
        // that company can only access their own products.

        public Task<List<Product>> GetAllProductAsync();

        public Task<Product?> GetProductByIdAsync(int id);

        public Task<bool> CreateProductAsync(Product product);

        public Task<Product?> UpdateProductAsync(Product product);

        public Task<bool> DeleteProductAsync(int id);

        public Task<List<Product>> SearchProductsByNameAsync(string name);
    }
}