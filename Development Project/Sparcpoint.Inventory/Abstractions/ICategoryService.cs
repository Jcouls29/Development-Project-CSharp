using Sparcpoint.Inventory.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Abstractions
{
    public interface ICategoryService
    {
        Task<int> CreateAsync(Category category, CancellationToken cancellationToken = default);
        Task<Category?> GetAsync(int instanceId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Category>> ListAsync(CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(Category category, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int instanceId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Category>> GetDescendantsAsync(int rootInstanceId, CancellationToken cancellationToken = default);
    }
}
