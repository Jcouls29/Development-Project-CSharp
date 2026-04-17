using Sparcpoint.Abstract.Repositories;
using Sparcpoint.Abstract.Services;
using Sparcpoint.DTOs;
using System;
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

        public async Task<int> AddProductAsync(CreateProductRequestDto request)
        {
            var productId = await _productRepository.AddProductAsync(request);
            if( productId <= 0 )
            {
                throw new InvalidOperationException("Failed to create product. The database did not return a valid Instance ID.");
            }
            return productId;
        }
    }
}
