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
    /// EVAL: Hand-rolled in-memory repo for tests. Exercises real service
    /// behavior without pulling in SQLite (which would lie about T-SQL dialect).
    public sealed class InMemoryProductRepository : IProductRepository
    {
        private readonly ConcurrentDictionary<int, Product> _products = new();
        private int _nextId = 0;

        public Task<int> AddAsync(Product product, CancellationToken cancellationToken = default)
        {
            if (product is null) throw new ArgumentNullException(nameof(product));

            var id = Interlocked.Increment(ref _nextId);
            var stored = new Product
            {
                InstanceId = id,
                Name = product.Name,
                Description = product.Description,
                ProductImageUris = product.ProductImageUris?.ToArray() ?? Array.Empty<string>(),
                ValidSkus = product.ValidSkus?.ToArray() ?? Array.Empty<string>(),
                CreatedTimestamp = DateTime.UtcNow,
                Attributes = product.Attributes?.Select(a => new ProductAttribute(a.Key, a.Value)).ToArray() ?? Array.Empty<ProductAttribute>(),
                CategoryIds = product.CategoryIds?.ToArray() ?? Array.Empty<int>(),
            };
            _products[id] = stored;
            return Task.FromResult(id);
        }

        public Task<Product?> GetByIdAsync(int instanceId, CancellationToken cancellationToken = default)
        {
            _products.TryGetValue(instanceId, out var product);
            return Task.FromResult(product);
        }

        public Task<IReadOnlyList<Product>> SearchAsync(ProductSearchCriteria criteria, CancellationToken cancellationToken = default)
        {
            if (criteria is null) throw new ArgumentNullException(nameof(criteria));

            IEnumerable<Product> query = _products.Values;

            if (!string.IsNullOrWhiteSpace(criteria.NameContains))
                query = query.Where(p => p.Name.Contains(criteria.NameContains, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(criteria.DescriptionContains))
                query = query.Where(p => p.Description.Contains(criteria.DescriptionContains, StringComparison.OrdinalIgnoreCase));

            if (criteria.CategoryIds.Count > 0)
                query = query.Where(p => criteria.CategoryIds.All(cid => p.CategoryIds.Contains(cid)));

            if (criteria.AttributeMatches.Count > 0)
            {
                query = query.Where(p => criteria.AttributeMatches.All(needle =>
                    p.Attributes.Any(a => a.Key.Equals(needle.Key, StringComparison.OrdinalIgnoreCase)
                                           && a.Value.Equals(needle.Value, StringComparison.OrdinalIgnoreCase))));
            }

            if (!string.IsNullOrWhiteSpace(criteria.Sku))
                query = query.Where(p => p.ValidSkus.Contains(criteria.Sku));

            var page = query
                .OrderBy(p => p.InstanceId)
                .Skip(criteria.Skip)
                .Take(criteria.Take)
                .ToArray();

            return Task.FromResult<IReadOnlyList<Product>>(page);
        }
    }
}
