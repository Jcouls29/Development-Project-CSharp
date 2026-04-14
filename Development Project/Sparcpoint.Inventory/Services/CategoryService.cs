using Sparcpoint.Inventory.Abstractions;
using Sparcpoint.Inventory.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Services
{
    public sealed class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;

        public CategoryService(ICategoryRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public Task<int> CreateAsync(Category category, CancellationToken cancellationToken = default)
        {
            Validate(category);
            return _repository.AddAsync(category, cancellationToken);
        }

        public Task<Category?> GetAsync(int instanceId, CancellationToken cancellationToken = default)
            => _repository.GetByIdAsync(instanceId, cancellationToken);

        public Task<IReadOnlyList<Category>> ListAsync(CancellationToken cancellationToken = default)
            => _repository.GetAllAsync(cancellationToken);

        public Task<bool> UpdateAsync(Category category, CancellationToken cancellationToken = default)
        {
            if (category is null) throw new ArgumentNullException(nameof(category));
            if (category.InstanceId <= 0) throw new ArgumentException("InstanceId required for update.", nameof(category));
            Validate(category);
            return _repository.UpdateAsync(category, cancellationToken);
        }

        public Task<bool> DeleteAsync(int instanceId, CancellationToken cancellationToken = default)
            => _repository.DeleteAsync(instanceId, cancellationToken);

        public Task<IReadOnlyList<Category>> GetDescendantsAsync(int rootInstanceId, CancellationToken cancellationToken = default)
            => _repository.GetDescendantsAsync(rootInstanceId, cancellationToken);

        private static void Validate(Category category)
        {
            if (category is null) throw new ArgumentNullException(nameof(category));
            if (string.IsNullOrWhiteSpace(category.Name)) throw new ArgumentException("Category.Name is required.", nameof(category));
            if (category.Name.Length > 64) throw new ArgumentException("Category.Name exceeds 64 characters.", nameof(category));
            if (category.Description?.Length > 256) throw new ArgumentException("Category.Description exceeds 256 characters.", nameof(category));

            foreach (var a in category.Attributes)
            {
                if (string.IsNullOrWhiteSpace(a.Key)) throw new ArgumentException("Attribute keys must be non-empty.", nameof(category));
                if (a.Key.Length > 64) throw new ArgumentException($"Attribute key '{a.Key}' exceeds 64 characters.", nameof(category));
                if (a.Value?.Length > 512) throw new ArgumentException($"Attribute '{a.Key}' value exceeds 512 characters.", nameof(category));
            }
        }
    }
}
