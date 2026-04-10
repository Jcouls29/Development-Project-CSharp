using Sparcpoint.Inventory.Models;

namespace Sparcpoint.Inventory.Repositories.InMemory
{
    /// <summary>
    /// EVAL: Thread-safe in-memory implementation of IProductRepository.
    /// Demonstrates the value of coding to interfaces — we can swap SqlServer
    /// for this implementation without changing any service or controller code.
    /// Useful for development, testing, and demo scenarios without a database.
    /// </summary>
    public class InMemoryProductRepository : IProductRepository
    {
        private readonly List<Product> _Products = new();
        private readonly object _Lock = new();
        private int _NextId = 1;

        public Task<Product> CreateAsync(Product product)
        {
            lock (_Lock)
            {
                product.InstanceId = _NextId++;
                product.CreatedTimestamp = DateTime.UtcNow;

                // EVAL: Store a deep copy to prevent external mutation of the in-memory store
                var stored = CloneProduct(product);
                _Products.Add(stored);
            }

            return Task.FromResult(product);
        }

        public Task<Product?> GetByIdAsync(int instanceId)
        {
            lock (_Lock)
            {
                var product = _Products.FirstOrDefault(p => p.InstanceId == instanceId);
                return Task.FromResult(product != null ? CloneProduct(product) : null);
            }
        }

        public Task<IEnumerable<Product>> SearchAsync(ProductSearchCriteria criteria)
        {
            lock (_Lock)
            {
                IEnumerable<Product> results = _Products;

                if (!string.IsNullOrWhiteSpace(criteria.NameContains))
                    results = results.Where(p => p.Name.Contains(criteria.NameContains, StringComparison.OrdinalIgnoreCase));

                if (!string.IsNullOrWhiteSpace(criteria.DescriptionContains))
                    results = results.Where(p => p.Description.Contains(criteria.DescriptionContains, StringComparison.OrdinalIgnoreCase));

                if (!string.IsNullOrWhiteSpace(criteria.SkuContains))
                    results = results.Where(p => p.ValidSkus.Contains(criteria.SkuContains, StringComparison.OrdinalIgnoreCase));

                if (criteria.CategoryIds?.Count > 0)
                    results = results.Where(p => p.CategoryIds.Any(c => criteria.CategoryIds.Contains(c)));

                if (criteria.Attributes?.Count > 0)
                {
                    foreach (var attr in criteria.Attributes)
                    {
                        results = results.Where(p =>
                            p.Attributes != null &&
                            p.Attributes.TryGetValue(attr.Key, out var val) &&
                            val == attr.Value);
                    }
                }

                return Task.FromResult<IEnumerable<Product>>(results.Select(CloneProduct).ToList());
            }
        }

        private static Product CloneProduct(Product source)
        {
            return new Product
            {
                InstanceId = source.InstanceId,
                Name = source.Name,
                Description = source.Description,
                ProductImageUris = source.ProductImageUris,
                ValidSkus = source.ValidSkus,
                CreatedTimestamp = source.CreatedTimestamp,
                Attributes = source.Attributes != null ? new Dictionary<string, string>(source.Attributes) : new(),
                CategoryIds = source.CategoryIds != null ? new List<int>(source.CategoryIds) : new()
            };
        }
    }
}
