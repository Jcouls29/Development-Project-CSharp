using Sparcpoint.Inventory.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory
{
    public interface IProductRepository
    {
        Task<int> AddAsync(ProductEntry product);
        Task<ProductEntry> GetByIdAsync(int instanceId);
        Task<IEnumerable<ProductEntry>> SearchAsync(ProductSearchFilter filter = null);
    }
}
