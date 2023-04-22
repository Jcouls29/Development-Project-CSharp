using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sparcpoint.Abstractions
{
    public interface IAsyncCollection<T> : IEnumerable<T>
    {
        Task<int> GetCount();
        Task Add(T item);
        Task Clear();
        Task<bool> Remove(T item);
    }
}
