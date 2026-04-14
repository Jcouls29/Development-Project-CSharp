using Sparcpoint.Inventory.Abstractions;
using Sparcpoint.Inventory.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Repositories.InMemory
{
    public sealed class InMemoryCategoryRepository : ICategoryRepository
    {
        private readonly ConcurrentDictionary<int, Category> _categories = new();
        private int _nextId = 0;

        public Task<int> AddAsync(Category category, CancellationToken cancellationToken = default)
        {
            if (category is null) throw new ArgumentNullException(nameof(category));
            var id = Interlocked.Increment(ref _nextId);
            _categories[id] = Clone(category, id, DateTime.UtcNow);
            return Task.FromResult(id);
        }

        public Task<Category?> GetByIdAsync(int instanceId, CancellationToken cancellationToken = default)
        {
            _categories.TryGetValue(instanceId, out var c);
            return Task.FromResult(c);
        }

        public Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Category>>(_categories.Values.OrderBy(c => c.InstanceId).ToArray());

        public Task<bool> UpdateAsync(Category category, CancellationToken cancellationToken = default)
        {
            if (!_categories.ContainsKey(category.InstanceId)) return Task.FromResult(false);
            var existing = _categories[category.InstanceId];
            _categories[category.InstanceId] = Clone(category, category.InstanceId, existing.CreatedTimestamp);
            return Task.FromResult(true);
        }

        public Task<bool> DeleteAsync(int instanceId, CancellationToken cancellationToken = default)
            => Task.FromResult(_categories.TryRemove(instanceId, out _));

        public Task<IReadOnlyList<Category>> GetDescendantsAsync(int rootInstanceId, CancellationToken cancellationToken = default)
        {
            // EVAL: BFS over ParentCategoryIds (child points up at parents).
            var result = new List<Category>();
            var queue = new Queue<int>();
            queue.Enqueue(rootInstanceId);
            var visited = new HashSet<int> { rootInstanceId };

            while (queue.Count > 0)
            {
                var parentId = queue.Dequeue();
                foreach (var c in _categories.Values.Where(c => c.ParentCategoryIds.Contains(parentId)))
                {
                    if (visited.Add(c.InstanceId))
                    {
                        result.Add(c);
                        queue.Enqueue(c.InstanceId);
                    }
                }
            }

            return Task.FromResult<IReadOnlyList<Category>>(result);
        }

        private static Category Clone(Category source, int id, DateTime created) => new()
        {
            InstanceId = id,
            Name = source.Name,
            Description = source.Description,
            CreatedTimestamp = created,
            Attributes = source.Attributes?.Select(a => new ProductAttribute(a.Key, a.Value)).ToArray() ?? Array.Empty<ProductAttribute>(),
            ParentCategoryIds = source.ParentCategoryIds?.ToArray() ?? Array.Empty<int>(),
        };
    }
}
