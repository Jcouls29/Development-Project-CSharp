using Sparcpoint.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interview.Web.Repositories.Interfaces
{
    // EVAL
    public interface IProductRepository
    {
        Task<Product> GetProductById(int id);
        
        Task<Product> CreateProduct(Product product);
        
        Task AddProduct(Product product);
        
        Task<IEnumerable<Product>> SearchProducts(string category, string name, string sku, string brand, string color);
    }
}