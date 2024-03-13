using Interview.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interview.Web.Service.Interfaces
{
    public interface IProductService
    {
        Task<List<Product>> GetProducts();
        Task<int> CreateProduct(Product product);
        Task<List<Product>> SearchProducts(string keyword);
    }
}
