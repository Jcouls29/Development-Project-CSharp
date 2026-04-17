using Sparcpoint;
using Sparcpoint.Inventory.Interfaces;
using Sparcpoint.Inventory.Models;
using System;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        }

        public async Task<ProductDetailModel> CreateProductAsync(CreateProductRequest request)
        {
            PreConditions.ParameterNotNull(request, nameof(request));
            PreConditions.StringNotNullOrWhitespace(request.Name, nameof(request.Name));
            PreConditions.StringNotNullOrWhitespace(request.Description, nameof(request.Description));

            var instanceId = await _productRepository.CreateAsync(request);
            return await _productRepository.GetByIdAsync(instanceId);
        }

        public Task<ProductDetailModel> GetProductAsync(int instanceId)
        {
            if (instanceId <= 0)
                throw new ArgumentOutOfRangeException(nameof(instanceId));

            return _productRepository.GetByIdAsync(instanceId);
        }

        public Task<PaginatedResult<ProductModel>> SearchProductsAsync(ProductSearchRequest request)
        {
            PreConditions.ParameterNotNull(request, nameof(request));

            if (request.Page <= 0)
                throw new ArgumentOutOfRangeException(nameof(request.Page));

            if (request.PageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(request.PageSize));

            return _productRepository.SearchAsync(request);
        }
    }
}
