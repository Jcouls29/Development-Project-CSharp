using Interview.Web.Models;
using Interview.Web.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interview.Web.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;

        public ProductService(IProductRepository repo)
        {
            _repo = repo;
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));
            if (string.IsNullOrWhiteSpace(product.Name))
                throw new ArgumentException("Product must have a name.", nameof(product));

            product.Id = product.Id == Guid.Empty ? Guid.NewGuid() : product.Id;
            product.CreatedAt = DateTime.UtcNow;
            product.Metadata = product.Metadata ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            return await _repo.AddAsync(product);
        }

        public Task<IEnumerable<Product>> GetAllAsync() => _repo.GetAllAsync();

        public Task<Product> GetByIdAsync(Guid id) => _repo.GetByIdAsync(id);

        public Task<IEnumerable<Product>> SearchByMetadataAsync(Dictionary<string, string> metadataCriteria)
        {
            return _repo.SearchByMetadataAsync(metadataCriteria);
        }

        public Task<IEnumerable<Product>> SearchAsync(string name, List<string> categories, Dictionary<string, string> metadataCriteria)
        {
            return _repo.SearchAsync(name, categories, metadataCriteria);
        }
    }
}
