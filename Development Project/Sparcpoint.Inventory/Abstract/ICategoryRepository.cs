using Sparcpoint.Inventory.Models;
using Sparcpoint.Inventory.Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Abstract
{
    public interface ICategoryRepository
    {
        Task<int> AddAsync(AddCategoryRequest request);
        Task<IEnumerable<Category>> GetAllAsync();
    }
}
