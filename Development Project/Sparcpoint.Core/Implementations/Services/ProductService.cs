using Interview.Web.Models;
using Sparcpoint.Abstract.Repositories;
using Sparcpoint.Abstract.Services;
using Sparcpoint.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sparcpoint.Implementations.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public Task<Product> AddAsync(Product product, CancellationToken ct = default)
            => this._productRepository.AddAsync(product);

        public Task<Product> GetByIdAsync(int id, CancellationToken ct = default)
            => this._productRepository.GetByIdAsync(id, ct);

        public Task<List<Product>> SearchAsync(SearchRequest search, CancellationToken ct = default)
            => this._productRepository.SearchAsync(search, ct);
    }
}
