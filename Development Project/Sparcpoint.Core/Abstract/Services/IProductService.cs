using System.Collections.Generic;
using System.Threading.Tasks;
using Sparcpoint.Core.Models;
using Sparcpoint.DTOs;

namespace Sparcpoint.Abstract.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllSync();
        Task<ProductDTO> Create(ProductDTO product);        
        Task<IEnumerable<Product>> SearchAsync(ProductDTO request);
    }
}