using Sparcpoint.Core.Entities;
using Sparcpoint.Core.QueryObjects;

namespace Sparcpoint.Core.Interfaces;

public interface IProductService
{
    public Task AddProductAsync(Product product);
    public Task UpdateProductAsync(Product product);
    public Task<Product?> GetProductByIdAsync(long id);
    public IAsyncEnumerable<Product> SearchProductsAsync(ProductSearchQuery searchQuery);
}
