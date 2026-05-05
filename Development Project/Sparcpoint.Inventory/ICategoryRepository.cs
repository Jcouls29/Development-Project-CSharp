using Sparcpoint.Inventory.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory
{
    public interface ICategoryRepository
    {
        Task<int> AddAsync(CategoryEntry category);
        Task<CategoryEntry> GetByIdAsync(int instanceId);
        Task<IEnumerable<CategoryEntry>> GetAllAsync();
    }
}
