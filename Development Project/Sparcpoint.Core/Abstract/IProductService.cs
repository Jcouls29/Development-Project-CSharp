using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sparcpoint.Core.Entities;

namespace Sparcpoint.Application.Abstracts
{
    public interface IProductService
    {
        Task<List<Product>> GetProducts();
        Task<List<Product>> GetProductsAsync();
        Task<int> CreateProduct(Product product);
        Task<int> CreateProductAsync(Product product);
        Task AddAttributesToProduct(int productId, List<ProductAttribute> productAttributes);
        Task AddAttributesToProductAsync(int productId, List<ProductAttribute> productAttributes);
        Task AddProductToCategories(int productId, List<ProductCategory> productCategories);
        Task AddProductToCategoriesAsync(int productId, List<ProductCategory> productCategories);
        Task<List<Product>> SearchProducts(string keyword);
        Task<List<Product>> SearchProductsAsync(string keyword);
        Task<List<ProductAttribute>> GetAttributesForProduct(int productId);
        Task<List<ProductAttribute>> GetAttributesForProductAsync(int productId);
    }
}