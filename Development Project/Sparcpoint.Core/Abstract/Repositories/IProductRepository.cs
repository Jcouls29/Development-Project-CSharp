using Sparcpoint.DTOs;
using System.Threading.Tasks;

namespace Sparcpoint.Abstract.Repositories
{
    public interface IProductRepository
    {
        Task<int> AddProductAsync(CreateProductRequestDto request);
    }
}
