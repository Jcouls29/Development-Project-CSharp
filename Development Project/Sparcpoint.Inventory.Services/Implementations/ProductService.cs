using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sparcpoint.Inventory.Models.Domain;
using Sparcpoint.Inventory.Models.DTOs.Requests;
using Sparcpoint.Inventory.Models.DTOs.Responses;
using Sparcpoint.Inventory.Models.Search;
using Sparcpoint.Inventory.Repositories.Interfaces;
using Sparcpoint.Inventory.Services.Interfaces;

namespace Sparcpoint.Inventory.Services.Implementations
{
    /// <summary>
    /// Product service implementation.
    /// EVAL: Encapsulates business logic and validation before delegating to repositories.
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly ILogger<ProductService> _logger;

        public ProductService(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IInventoryRepository inventoryRepository,
            ILogger<ProductService> logger)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _inventoryRepository = inventoryRepository ?? throw new ArgumentNullException(nameof(inventoryRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ProductResponse> CreateProductAsync(CreateProductRequest request)
        {
            // EVAL: Validate business rules before creating product
            await ValidateCreateProductRequest(request);

            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                ProductImageUris = request.ProductImageUris ?? new List<string>(),
                ValidSkus = request.ValidSkus ?? new List<string>(),
                Attributes = request.Attributes ?? new Dictionary<string, string>(),
                CategoryIds = request.CategoryIds ?? new List<int>()
            };

            var createdProduct = await _productRepository.CreateProductAsync(product);
            
            _logger.LogInformation("Product {ProductId} created successfully", createdProduct.InstanceId);
            
            return MapToProductResponse(createdProduct);
        }

        public async Task<ProductResponse?> GetProductByIdAsync(int instanceId)
        {
            var product = await _productRepository.GetProductByIdAsync(instanceId);
            return product != null ? MapToProductResponse(product) : null;
        }

        public async Task<List<ProductResponse>> SearchProductsAsync(ProductSearchCriteria criteria)
        {
            // EVAL: Apply default pagination if not specified to prevent large result sets
            criteria.Limit ??= 100;
            criteria.Offset ??= 0;

            var products = await _productRepository.SearchProductsAsync(criteria);
            
            _logger.LogInformation("Product search returned {Count} results", products.Count);
            
            return products.Select(MapToProductResponse).ToList();
        }

        public async Task<InventoryCountResponse?> GetProductInventoryAsync(int instanceId)
        {
            var criteria = new InventoryCountCriteria
            {
                ProductInstanceId = instanceId,
                OnlyCompleted = true
            };

            var results = await _inventoryRepository.GetInventoryCountAsync(criteria);
            return results.FirstOrDefault();
        }

        #region Validation

        private async Task ValidateCreateProductRequest(CreateProductRequest request)
        {
            // EVAL: Validate category IDs exist before creating product
            if (request.CategoryIds != null && request.CategoryIds.Any())
            {
                var categories = await _categoryRepository.GetCategoriesByIdsAsync(request.CategoryIds);
                var missingCategories = request.CategoryIds.Except(categories.Select(c => c.InstanceId)).ToList();
                
                if (missingCategories.Any())
                {
                    throw new InvalidOperationException(
                        $"The following category IDs do not exist: {string.Join(", ", missingCategories)}");
                }
            }

            // EVAL: Validate attribute keys/values don't exceed database limits
            if (request.Attributes != null)
            {
                foreach (var attr in request.Attributes)
                {
                    if (attr.Key.Length > 64)
                        throw new InvalidOperationException($"Attribute key '{attr.Key}' exceeds maximum length of 64 characters");
                    
                    if (attr.Value.Length > 512)
                        throw new InvalidOperationException($"Attribute value for key '{attr.Key}' exceeds maximum length of 512 characters");
                }
            }
        }

        #endregion

        #region Mapping

        private ProductResponse MapToProductResponse(Product product)
        {
            return new ProductResponse
            {
                InstanceId = product.InstanceId,
                Name = product.Name,
                Description = product.Description,
                ProductImageUris = product.ProductImageUris,
                ValidSkus = product.ValidSkus,
                Attributes = product.Attributes,
                CategoryIds = product.CategoryIds,
                CreatedTimestamp = product.CreatedTimestamp
            };
        }

        #endregion
    }
}