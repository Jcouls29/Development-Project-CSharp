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

        public async Task<IEnumerable<Product>> SearchAsync(IEnumerable<int> categoryIds, string attrKey, string attrValue)
        {
            return await _productRepository.SearchAsync(categoryIds, attrKey, attrValue);
        }
    }
}
