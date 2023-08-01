using Interview.Web.Dtos;
using Interview.Web.Models;
using System.Threading.Tasks;

namespace Interview.Web.Data
{
    public interface IInventoryRepo
    {
        public Task<Product> CreateProduct(ProductCreateDto product);
        public Task<Product> GetProductById(int id);

    }
}
