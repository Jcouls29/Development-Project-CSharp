using Sparcpoint.Inventory.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Abstract
{
    public interface ICategoryRepository
    {
        Task<Category> GetByIdAsync(int categoryId);
        Task<IEnumerable<Category>> GetAllAsync();
        Task<int> CreateAsync(CreateCategoryRequest request);
    }
}
