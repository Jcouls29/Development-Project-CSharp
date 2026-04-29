using Sparcpoint.Abstract.Repositories;
using Sparcpoint.Abstract.Services;
using Sparcpoint.Models.DTOs;
using Sparcpoint.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new ArgumentException("Product name cannot be empty.", nameof(request.Name));
            }
            var prodID = await _productRepository.AddProductAsync(request);
            if (prodID <= 0)
            {
                throw new InvalidOperationException("Unable to create Product - No Prod id returned");
            }
            return prodID;
        }

        public async Task<IEnumerable<Product>> SearchProductAsync(SearchProductRequestDto request)
        {
            var products = await _productRepository.SearchProductAsync(request);

            return products;
        }

        public async Task<Product> GetProductByIdAsync(int productId)
        {
            var product = await _productRepository.GetProductByIdAsync(productId);
            if (product == null)
            {
                throw new KeyNotFoundException($"No product found with id {productId}");
            }
            return product;
        }
    }
}
