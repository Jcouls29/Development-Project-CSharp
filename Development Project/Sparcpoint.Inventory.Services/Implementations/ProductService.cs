using Microsoft.Extensions.Logging;
using Sparcpoint.Inventory.Models;
using Sparcpoint.Inventory.Repositories.Contracts;
using Sparcpoint.Inventory.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly ILogger<ProductService> _logger;
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository, ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<int> AddProduct(Product product)
        {
            try
            {
                int productId = 0;
                productId = await _productRepository.AddProduct(product);
                return productId;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.Message);
                throw;
            }
        }
    }
}
