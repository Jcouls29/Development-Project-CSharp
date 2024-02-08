using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Sparcpoint
{
    public class BlockingCollectionAsyncQueue<T> : IAsyncQueue<T>, IDisposable
    {
        private ConcurrentQueue<T> _Entries;
        private TimeSpan _DequeueTimeout;

        public BlockingCollectionAsyncQueue(TimeSpan dequeueTimeout)
        {
            if (dequeueTimeout <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(dequeueTimeout), "Dequeue Timeout must be greater than zero.");

            _DequeueTimeout = dequeueTimeout;
            _Entries = new ConcurrentQueue<T>(new ConcurrentQueue<T>());
        }

        public void Add(T entry)
        {
            if (entry == null)
                throw new ArgumentNullException(nameof(entry));

            _Entries.Enqueue(entry);
        }

        public async Task<IEnumerable<T>> Take(int count, CancellationToken cancelToken)
        {
            Stopwatch watch = Stopwatch.StartNew();

            List<T> results = new List<T>();
            while (results.Count < count && watch.Elapsed < _DequeueTimeout && !cancelToken.IsCancellationRequested)
            {
                if (_Entries.TryDequeue(out T entry))
                    results.Add(entry);
                else
                    await Task.Delay(100, cancelToken);
            }

            return results;
        }

        public void Dispose()
        {
            // Nothing to dispose right now
        }
    }
}
