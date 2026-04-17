using System.Collections.Generic;
using System.Threading.Tasks;
using Sparcpoint.Abstract.Repositories;
using Sparcpoint.Abstract.Services;
using Sparcpoint.Core.Models;
using Sparcpoint.DTOs;

namespace Sparcpoint.Implementations.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        /*
         * Create new product
         */
        public async Task<ProductDTO> Create(ProductDTO product)
        {
            var productCreated = await _productRepository.Create(product);

            var toReturn = new ProductDTO
            {
                Categories = productCreated.Categories
                //...
            };


            return toReturn;
        }

        /*
         * Gets all products
         */
        public async Task<IEnumerable<Product>> GetAllSync()
        {
            return await _productRepository.GetAllAsync();
        }

        public Task<IEnumerable<Product>> SearchAsync(ProductDTO request)
        {
            return _productRepository.SearchAsync(request);
        }
    }
}
