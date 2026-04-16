using Sparcpoint.Domain;
using System.Threading.Tasks;

namespace Sparcpoint.Abstract.Services
{
    public interface IProductService
    {
        Task<int> AddProductAsync(Product request);

    }
}
