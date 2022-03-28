using Interview.Web.CustomModels;
using Interview.Web.Models;
using System.Threading.Tasks;

namespace Interview.Web.Interfaces
{
    public interface IProductRepo
    {
        Task<object> SearchProducts(SearchInput input);
        Task<bool> AddOrUpdateProduct(Product product);
        Task<bool> RemoveProduct(int instanceId);
    }
}
