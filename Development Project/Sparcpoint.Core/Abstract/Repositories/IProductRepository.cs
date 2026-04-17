using System.Collections.Generic;
using System.Threading.Tasks;
using Sparcpoint.Core.Models;
using Sparcpoint.DTOs;

namespace Sparcpoint.Abstract.Repositories
{
    public interface IProductRepository
    {
        Task<Product> Create(ProductDTO product);
        Task<IEnumerable<Product>> GetAllAsync();
        Task<IEnumerable<Product>> SearchAsync(ProductDTO request);
    }
}
