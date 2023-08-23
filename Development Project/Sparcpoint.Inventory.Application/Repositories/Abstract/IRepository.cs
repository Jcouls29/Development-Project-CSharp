using Sparcpoint.Inventory.Domain.Entities.Instances;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Application.Repositories
{
    public interface IRepository<T>
    {
        Task<T> GetAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> AddAsync(T item);
        Task<bool> RemoveAsync(int id);
    }
}
