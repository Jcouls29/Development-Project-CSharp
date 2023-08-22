using System.Threading.Tasks;
using Interview.Web.Models;

namespace Interview.Web
{
    public interface IProductDB
    {
        Task<int> AddProductAsync(Product product);
    }
}