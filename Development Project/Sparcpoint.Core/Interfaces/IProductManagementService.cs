using Sparcpoint.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    public interface IProductManagementService
    {
        Task AddProductAsync(Product product);
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product> GetProductAsync(string name);
    }
}