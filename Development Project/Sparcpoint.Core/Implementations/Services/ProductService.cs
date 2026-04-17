using Sparcpoint.Abstract.Repositories;
using Sparcpoint.Abstract.Services;
using Sparcpoint.Models;
using Sparcpoint.Request;
using System;
using System.Collections.Generic;
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

        public async Task<int> CreateAsync(CreateProductRequest createProductRequest)
        {
            PreConditions.ParameterNotNull(createProductRequest, nameof(createProductRequest));
            PreConditions.StringNotNullOrWhitespace(createProductRequest.Name, nameof(createProductRequest.Name));
            PreConditions.StringNotNullOrWhitespace(createProductRequest.Description, nameof(createProductRequest.Description));

            return await _productRepository.CreateAsync(createProductRequest);                     
        }

        public Task<ProductDetail> GetByIdAsync(int instanceId)
        {
            if (instanceId <= 0)
                throw new ArgumentOutOfRangeException(nameof(instanceId));

            return _productRepository.GetByIdAsync(instanceId);
        }
     
        public Task<List<ProductDetail>> SearchAsync(ProductSearchRequest ProductSearchRequest)
        {
            PreConditions.ParameterNotNull(ProductSearchRequest, nameof(ProductSearchRequest));                     

            return _productRepository.SearchAsync(ProductSearchRequest);
        }
    }
}
