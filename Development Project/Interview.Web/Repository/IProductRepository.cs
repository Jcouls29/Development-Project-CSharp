using Interview.Web.Model;
using Sparcpoint;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interview.Web.Repository
{
    public interface IProductRepository :IAsyncCollection<Product>
    {
        Task<List<Product>> SearchProduct(Product product);
    }
}