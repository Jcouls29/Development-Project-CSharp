using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Abstractions
{
    public interface ICategoryRepository
    {
        /// <summary>
        /// Creates a category, optionally linking it to parent categories.
        /// Returns the new category's InstanceId.
        /// </summary>
        Task<int> AddAsync(CreateCategoryRequest request);

        /// <summary>
        /// Returns all categories in the system.
        /// </summary>
        Task<IEnumerable<Category>> GetAllAsync();
    }
}
