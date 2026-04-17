using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sparcpoint.Inventory.Models;

namespace Sparcpoint.Inventory.Repositories
{
    /// <summary>
    /// EVAL: Abstraction for categories. Intentionally kept small
    /// (basic CRUD + list) to avoid bloating the contract. Future features
    /// (e.g. category search by metadata) can be added as
    /// separate interfaces following the Interface Segregation Principle.
    /// </summary>
    public interface ICategoryRepository
    {
        Task<int> AddAsync(Category category, CancellationToken cancellationToken = default);
        Task<Category> GetByIdAsync(int instanceId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Category>> ListAsync(CancellationToken cancellationToken = default);
    }
}
