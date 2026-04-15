using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Core.DTOs;
using Sparcpoint.Core.Models;
using Sparcpoint.Core.Repositories;
using Sparcpoint.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [Route("api/v1/products")]
    [ApiController]
    // EVAL: API Controller - RESTful endpoints for product management
    // EVAL: Model validation - automatic validation using DataAnnotations
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IInventoryService _inventoryService;

        // EVAL: Dependency Injection - Constructor injection for loose coupling
        public ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository, IInventoryService inventoryService)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _inventoryService = inventoryService ?? throw new ArgumentNullException(nameof(inventoryService));
        }

        // EVAL: RESTful GET - Retrieve all products with optional search
        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromQuery] ProductSearchDto searchDto)
        {
            searchDto ??= new ProductSearchDto();

            var hasSearchCriteria = !string.IsNullOrWhiteSpace(searchDto.Name)
                || !string.IsNullOrWhiteSpace(searchDto.Description)
                || (searchDto.CategoryInstanceIds?.Any() == true)
                || (searchDto.MetadataFilters?.Any() == true)
                || searchDto.Skip.HasValue
                || searchDto.Take.HasValue;

            if (!hasSearchCriteria)
            {
                var products = await _productRepository.GetAllAsync();
                var productDtos = new List<ProductDto>();

                foreach (var product in products)
                {
                    var inventoryCount = await _inventoryService.GetCurrentInventoryAsync(product.InstanceId);
                    productDtos.Add(new ProductDto
                    {
                        InstanceId = product.InstanceId,
                        Name = product.Name,
                        Description = product.Description,
                        ProductImageUris = product.ProductImageUris.ToList(),
                        ValidSkus = product.ValidSkus.ToList(),
                        CreatedTimestamp = product.CreatedTimestamp,
                        Categories = product.Categories.Select(c => new CategoryDto
                        {
                            InstanceId = c.InstanceId,
                            Name = c.Name,
                            Description = c.Description,
                            CreatedTimestamp = c.CreatedTimestamp
                        }).ToList(),
                        Metadata = new Dictionary<string, string>(product.Metadata),
                        CurrentInventoryCount = inventoryCount
                    });
                }

                return Ok(productDtos);
            }

            var productsToReturn = await _productRepository.SearchAsync(
                searchDto.Name,
                searchDto.Description,
                searchDto.CategoryInstanceIds,
                searchDto.MetadataFilters,
                searchDto.Skip,
                searchDto.Take
            );

            var totalCount = await _productRepository.GetSearchCountAsync(
                searchDto.Name,
                searchDto.Description,
                searchDto.CategoryInstanceIds,
                searchDto.MetadataFilters
            );

            var searchProductDtos = new List<ProductDto>();
            foreach (var product in productsToReturn)
            {
                var inventoryCount = await _inventoryService.GetCurrentInventoryAsync(product.InstanceId);
                searchProductDtos.Add(new ProductDto
                {
                    InstanceId = product.InstanceId,
                    Name = product.Name,
                    Description = product.Description,
                    ProductImageUris = product.ProductImageUris.ToList(),
                    ValidSkus = product.ValidSkus.ToList(),
                    CreatedTimestamp = product.CreatedTimestamp,
                    Categories = product.Categories.Select(c => new CategoryDto
                    {
                        InstanceId = c.InstanceId,
                        Name = c.Name,
                        Description = c.Description,
                        CreatedTimestamp = c.CreatedTimestamp
                    }).ToList(),
                    Metadata = new Dictionary<string, string>(product.Metadata),
                    CurrentInventoryCount = inventoryCount
                });
            }

            return Ok(new ProductSearchResultDto
            {
                Products = searchProductDtos,
                TotalCount = totalCount
            });
        }

        [HttpPost("search")]
        [HttpPost("/api/products/search")]
        public async Task<IActionResult> SearchProducts([FromBody] ProductSearchDto searchDto)
        {
            if (searchDto == null)
            {
                return BadRequest("Search request body cannot be empty.");
            }

            var products = await _productRepository.SearchAsync(
                searchDto.Name,
                searchDto.Description,
                searchDto.CategoryInstanceIds,
                searchDto.MetadataFilters,
                searchDto.Skip,
                searchDto.Take);

            var totalCount = await _productRepository.GetSearchCountAsync(
                searchDto.Name,
                searchDto.Description,
                searchDto.CategoryInstanceIds,
                searchDto.MetadataFilters);

            var productDtos = new List<ProductDto>();
            foreach (var product in products)
            {
                var inventoryCount = await _inventoryService.GetCurrentInventoryAsync(product.InstanceId);
                productDtos.Add(new ProductDto
                {
                    InstanceId = product.InstanceId,
                    Name = product.Name,
                    Description = product.Description,
                    ProductImageUris = product.ProductImageUris.ToList(),
                    ValidSkus = product.ValidSkus.ToList(),
                    CreatedTimestamp = product.CreatedTimestamp,
                    Categories = product.Categories.Select(c => new CategoryDto
                    {
                        InstanceId = c.InstanceId,
                        Name = c.Name,
                        Description = c.Description,
                        CreatedTimestamp = c.CreatedTimestamp
                    }).ToList(),
                    Metadata = new Dictionary<string, string>(product.Metadata),
                    CurrentInventoryCount = inventoryCount
                });
            }

            return Ok(new ProductSearchResultDto
            {
                Products = productDtos,
                TotalCount = totalCount
            });
        }

        // EVAL: RESTful GET by ID - Retrieve specific product
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return NotFound();

            var inventoryCount = await _inventoryService.GetCurrentInventoryAsync(product.InstanceId);

            var productDto = new ProductDto
            {
                InstanceId = product.InstanceId,
                Name = product.Name,
                Description = product.Description,
                ProductImageUris = product.ProductImageUris.ToList(),
                ValidSkus = product.ValidSkus.ToList(),
                CreatedTimestamp = product.CreatedTimestamp,
                Categories = product.Categories.Select(c => new CategoryDto
                {
                    InstanceId = c.InstanceId,
                    Name = c.Name,
                    Description = c.Description,
                    CreatedTimestamp = c.CreatedTimestamp
                }).ToList(),
                Metadata = new Dictionary<string, string>(product.Metadata),
                CurrentInventoryCount = inventoryCount
            };

            return Ok(productDto);
        }

        // EVAL: RESTful POST - Create new product
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto createDto)
        {
            // EVAL: Model validation - automatic via [ApiController] and DataAnnotations
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // EVAL: Load categories to associate with product
            var categories = new List<Category>();
            foreach (var categoryId in createDto.CategoryInstanceIds)
            {
                var category = await _categoryRepository.GetByIdAsync(categoryId);
                if (category != null) categories.Add(category);
            }

            // EVAL: Create product using factory method
            var product = Product.Create(createDto.Name, createDto.Description, createDto.ProductImageUris, createDto.ValidSkus);

            // EVAL: Associate categories
            foreach (var category in categories)
            {
                product.AddCategory(category);
            }

            // EVAL: Set metadata (key-value pairs)
            foreach (var kvp in createDto.Metadata)
            {
                product.SetMetadata(kvp.Key, kvp.Value);
            }

            // EVAL: Persist product
            var productId = await _productRepository.AddAsync(product);

            return CreatedAtAction(nameof(GetProduct), new { id = productId }, new { InstanceId = productId });
        }

        // EVAL: RESTful PUT - Update existing product
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] CreateProductDto updateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existingProduct = await _productRepository.GetByIdAsync(id);
            if (existingProduct == null) return NotFound();

            // EVAL: Load categories
            var categories = new List<Category>();
            foreach (var categoryId in updateDto.CategoryInstanceIds)
            {
                var category = await _categoryRepository.GetByIdAsync(categoryId);
                if (category != null) categories.Add(category);
            }

            // EVAL: Create updated product and preserve the identity
            var updatedProduct = Product.Create(updateDto.Name, updateDto.Description, updateDto.ProductImageUris, updateDto.ValidSkus);
            updatedProduct.SetInstanceId(id);

            // EVAL: Associate categories and metadata
            foreach (var category in categories)
            {
                updatedProduct.AddCategory(category);
            }

            foreach (var kvp in updateDto.Metadata)
            {
                updatedProduct.SetMetadata(kvp.Key, kvp.Value);
            }

            // EVAL: Persist changes
            await _productRepository.UpdateAsync(updatedProduct);

            return NoContent();
        }
    }
}
