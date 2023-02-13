using Sparcpoint.Inventory.Models;

namespace Sparcpoint.Inventory.Abstract
{
    public interface IProductService
    {
        int CreateProduct(Product product);
        Task<int> CreateProductAsync(Product product);

        IEnumerable<Product> GetProducts();
        Task<IEnumerable<Product>> GetProductsAsync();

        void AddAttributesToProduct(int productId, IEnumerable<ProductAttribute> productAttributes);
        Task AddAttributesToProductAsync(int productId, IEnumerable<ProductAttribute> productAttributes);

        void AddProductToCategories(int productId, IEnumerable<ProductCategory> productCategories);
        Task AddProductToCategoriesAsync(int productId, IEnumerable<ProductCategory> productCategories);

        IEnumerable<Product> SearchProducts(string keyword, ProductSearchScope searchScope);
        Task<IEnumerable<Product>> SearchProductsAsync(string keyword, ProductSearchScope searchScope);
    }
}
