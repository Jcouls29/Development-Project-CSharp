using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sparcpoint
{
    public interface IAsyncQueue<T>
    {
        void Add(T entry);
        Task<IEnumerable<T>> Take(int count, CancellationToken cancelToken);
    }
}
