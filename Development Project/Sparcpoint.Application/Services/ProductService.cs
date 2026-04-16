using Sparcpoint.Application.DTOs;
using Sparcpoint.Application.Interfaces;
using Sparcpoint.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<int> CreateProduct(CreateProductRequest request)
        {
            PreConditions.ParameterNotNull(request, nameof(request));
            PreConditions.StringNotNullOrWhitespace(request.Name, nameof(request.Name));


            var productId = await _productRepository.InsertProduct(request);

            return productId;
        }

        public async Task<IEnumerable<Product>> SearchProducts(ProductSearchRequest request)
        {
            PreConditions.ParameterNotNull(request, nameof(request));



            return await _productRepository.SearchProducts(request);
        }
    }
}
