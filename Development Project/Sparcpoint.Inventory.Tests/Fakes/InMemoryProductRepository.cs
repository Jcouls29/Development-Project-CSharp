// EVAL: In-memory fake implementation of IProductRepository.
// This proves the interface is implementable without any database
// and demonstrates the open-closed principle -- new data store,
// same interface, zero changes to controllers or tests.

using Sparcpoint.Inventory.Abstract;
using Sparcpoint.Inventory.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Tests.Fakes
{
    /// <summary>
    /// In-memory implementation of IProductRepository for testing.
    /// Uses ConcurrentDictionary for thread-safe storage.
    /// </summary>
    public class InMemoryProductRepository : IProductRepository
    {
        private readonly ConcurrentDictionary<int, Product> _products = new();
        private int _nextId = 1;

        /// <summary>
        /// Pre-seeds a product into the store. Used for test setup.
        /// </summary>
        public void Seed(Product product)
        {
            if (product.InstanceId == 0)
                product.InstanceId = Interlocked.Increment(ref _nextId);

            _products[product.InstanceId] = product;
        }

        public Task<Product> CreateAsync(Product product, CancellationToken cancellationToken = default)
        {
            product.InstanceId = Interlocked.Increment(ref _nextId);
            product.CreatedTimestamp = DateTime.UtcNow;

            // Deep copy attributes and categories to simulate DB isolation
            var stored = new Product
            {
                InstanceId = product.InstanceId,
                Name = product.Name,
                Description = product.Description,
                ProductImageUris = new List<string>(product.ProductImageUris ?? new List<string>()),
                ValidSkus = new List<string>(product.ValidSkus ?? new List<string>()),
                Attributes = new Dictionary<string, string>(product.Attributes ?? new Dictionary<string, string>()),
                CategoryIds = new List<int>(product.CategoryIds ?? new List<int>()),
                Categories = new Dictionary<int, string>(product.Categories ?? new Dictionary<int, string>()),
                CreatedTimestamp = product.CreatedTimestamp
            };

            _products[stored.InstanceId] = stored;
            return Task.FromResult(stored);
        }

        public Task<Product> GetByIdAsync(int instanceId, CancellationToken cancellationToken = default)
        {
            _products.TryGetValue(instanceId, out var product);
            return Task.FromResult(product);
        }

        public Task<IEnumerable<Product>> SearchAsync(ProductSearchCriteria criteria, CancellationToken cancellationToken = default)
        {
            criteria ??= new ProductSearchCriteria();

            var query = _products.Values.AsEnumerable();

            // Name filter (partial, case-insensitive)
            if (!string.IsNullOrWhiteSpace(criteria.Name))
                query = query.Where(p => p.Name != null && p.Name.Contains(criteria.Name, StringComparison.OrdinalIgnoreCase));

            // Description filter
            if (!string.IsNullOrWhiteSpace(criteria.Description))
                query = query.Where(p => p.Description != null && p.Description.Contains(criteria.Description, StringComparison.OrdinalIgnoreCase));

            // Category filter (product in ANY of the specified categories)
            if (criteria.CategoryIds != null && criteria.CategoryIds.Count > 0)
                query = query.Where(p => p.CategoryIds != null && p.CategoryIds.Intersect(criteria.CategoryIds).Any());

            // Attribute filter (ALL specified key-value pairs must match)
            if (criteria.Attributes != null && criteria.Attributes.Count > 0)
            {
                foreach (var attr in criteria.Attributes)
                {
                    query = query.Where(p =>
                        p.Attributes != null &&
                        p.Attributes.TryGetValue(attr.Key, out var val) &&
                        val == attr.Value);
                }
            }

            // Pagination
            var results = query
                .OrderBy(p => p.Name)
                .Skip(criteria.Skip)
                .Take(criteria.Take);

            return Task.FromResult(results);
        }
    }
}
