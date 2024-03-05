using Microsoft.EntityFrameworkCore;
using Sparcpoint.Core.Entities;
using Sparcpoint.Core.Interfaces;
using Sparcpoint.Core.QueryObjects;
using Sparcpoint.Infrastructure.Data;

namespace Sparcpoint.Infrastructure.Services;

public class ProductService(ApplicationDbContext dbContext) : IProductService
{
    public Task AddProductAsync(Product product)
    {
        dbContext.Products.Add(product);
        return dbContext.SaveChangesAsync();
    }

    public Task UpdateProductAsync(Product product)
    {
        return dbContext.SaveChangesAsync();
    }

    public Task<Product?> GetProductByIdAsync(long id)
    {
        return dbContext.Products
            .Include(p => p.Attributes)
            .Include(p => p.Categories)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public IAsyncEnumerable<Product> SearchProductsAsync(ProductSearchQuery searchQuery)
    {
        return dbContext.Products
            .Where(p => searchQuery.Attributes == null || p.Attributes.Any(a => a.Value != null && searchQuery.Attributes[a.Name].Equals(a.Value, StringComparison.CurrentCultureIgnoreCase)))
            .Where(p => searchQuery.Categories == null || p.Categories.Any(a => searchQuery.Categories.Any(c => c.Equals(a.Name, StringComparison.CurrentCultureIgnoreCase))))
            .Where(p => searchQuery.ProductName == null || p.Name.ToLower().Contains(searchQuery.ProductName.ToLower()))
            .Where(p => searchQuery.ProductDescription == null || p.Description.ToLower().Contains(searchQuery.ProductDescription.ToLower()))
            .Where(p => searchQuery.ProductImageUris == null || p.ImageUris.ToLower().Contains(searchQuery.ProductImageUris.ToLower()))
            .Where(p => searchQuery.ProductValidSkus == null || p.ValidSkus.ToLower().Contains(searchQuery.ProductValidSkus.ToLower()))
            .AsAsyncEnumerable();
    }
}
