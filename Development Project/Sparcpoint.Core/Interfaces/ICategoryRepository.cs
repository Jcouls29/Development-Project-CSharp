using Sparcpoint.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sparcpoint.Interfaces
{
    public interface ICategoryRepository
    {
        Task<int> CreateAsync(Category category);

        Task<Category> GetByIdAsync(int instanceId);

        Task<IEnumerable<Category>> GetAllAsync();
    }
}
