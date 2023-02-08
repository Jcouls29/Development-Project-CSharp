using Sparcpoint.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sparcpoint.Interfaces
{
    public interface IProductsRepository
    {
            Task<IEnumerable<Product>> GetProductsAsync();
    }
}
