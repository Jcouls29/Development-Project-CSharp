using Sparcpoint.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sparcpoint
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<int> CreateAsync(Product product)
        {
            return await _productRepository.AddAsync(product);
        }

        public async Task<IEnumerable<Product>> SearchAsync(ProductSearchRequest request)
        {
            return await _productRepository.SearchAsync(request);
        }

        public async Task<IEnumerable<Product>> GetAll()
        {
            // NOTE: Sample method to demonstrate how to add additional service methods that may not be in the repository interface
            return await _productRepository.GetAll();
        }
    }
}
