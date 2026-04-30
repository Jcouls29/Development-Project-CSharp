using Sparcpoint.Inventory.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Abstract
{
    // EVAL: Generic base repository interface ensures all future entities get
    // a consistent CRUD contract for free. The open-closed principle applies here:
    // new entity types extend this without modifying it or any dependent code.
    public interface IInstanceRepository<T> where T : InstanceBase
    {
        Task<T?> GetByIdAsync(int instanceId);
        Task<IEnumerable<T>> GetAllAsync();
        Task<int> AddAsync(T instance);
    }
}
