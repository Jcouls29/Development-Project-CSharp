using Sparcpoint.Inventory.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Abstractions
{
    public interface ICategoryRepository
    {
        Task<int> AddAsync(Category category, CancellationToken cancellationToken = default);
        Task<Category?> GetByIdAsync(int instanceId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(Category category, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int instanceId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Category>> GetDescendantsAsync(int rootInstanceId, CancellationToken cancellationToken = default);
    }
}
