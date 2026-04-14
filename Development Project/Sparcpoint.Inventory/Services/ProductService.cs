using Sparcpoint.Inventory.Abstractions;
using Sparcpoint.Inventory.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Services
{
    public sealed class ProductService : IProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public Task<int> AddProductAsync(Product product, CancellationToken cancellationToken = default)
        {
            if (product is null) throw new ArgumentNullException(nameof(product));
            if (string.IsNullOrWhiteSpace(product.Name)) throw new ArgumentException("Product.Name is required.", nameof(product));
            // EVAL: Matches VARCHAR(256) column width to surface validation errors before hitting SQL.
            if (product.Name.Length > 256) throw new ArgumentException("Product.Name exceeds 256 characters.", nameof(product));
            if (product.Description?.Length > 256) throw new ArgumentException("Product.Description exceeds 256 characters.", nameof(product));

            foreach (var attribute in product.Attributes)
            {
                if (string.IsNullOrWhiteSpace(attribute.Key)) throw new ArgumentException("Attribute keys must be non-empty.", nameof(product));
                if (attribute.Key.Length > 64) throw new ArgumentException($"Attribute key '{attribute.Key}' exceeds 64 characters.", nameof(product));
                if (attribute.Value?.Length > 512) throw new ArgumentException($"Attribute '{attribute.Key}' value exceeds 512 characters.", nameof(product));
            }

            return _repository.AddAsync(product, cancellationToken);
        }

        public Task<Product?> GetProductAsync(int instanceId, CancellationToken cancellationToken = default)
            => _repository.GetByIdAsync(instanceId, cancellationToken);

        public Task<IReadOnlyList<Product>> SearchAsync(ProductSearchCriteria criteria, CancellationToken cancellationToken = default)
        {
            if (criteria is null) throw new ArgumentNullException(nameof(criteria));
            if (criteria.Take <= 0 || criteria.Take > 500) criteria.Take = 50;
            if (criteria.Skip < 0) criteria.Skip = 0;
            return _repository.SearchAsync(criteria, cancellationToken);
        }
    }
}
