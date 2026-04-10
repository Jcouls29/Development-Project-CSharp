using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asp.Versioning;
using Interview.Web.DTOs;
using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Inventory.Models;
using Sparcpoint.Inventory.Services;

namespace Interview.Web.Controllers
{
    /// <summary>
    /// EVAL: Product API controller providing endpoints for creating and searching products.
    /// Products can be added but never deleted, per business requirements.
    /// Exception handling is centralized in ErrorHandlingMiddleware, keeping controllers thin.
    /// </summary>
    [ApiVersion(1.0)]
    [Route("api/v{version:apiVersion}/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _ProductService;

        public ProductController(IProductService productService)
        {
            _ProductService = productService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
        {
            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                ProductImageUris = request.ProductImageUris != null ? string.Join(",", request.ProductImageUris) : string.Empty,
                ValidSkus = request.ValidSkus != null ? string.Join(",", request.ValidSkus) : string.Empty,
                Attributes = request.Attributes ?? new Dictionary<string, string>(),
                CategoryIds = request.CategoryIds ?? new List<int>()
            };

            var created = await _ProductService.CreateProductAsync(product);
            var response = ProductResponse.FromProduct(created);

            return CreatedAtAction(nameof(GetProduct), new { id = response.InstanceId }, response);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _ProductService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound(new { error = $"Product with ID {id} not found." });

            return Ok(ProductResponse.FromProduct(product));
        }

        /// <summary>
        /// EVAL: Searches for products by any combination of name, description, SKU, categories, and attributes.
        /// All query parameters are optional — omitting all returns all products.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> SearchProducts([FromQuery] SearchProductsRequest request)
        {
            var criteria = new ProductSearchCriteria
            {
                NameContains = request.NameContains,
                DescriptionContains = request.DescriptionContains,
                SkuContains = request.SkuContains
            };

            if (!string.IsNullOrWhiteSpace(request.CategoryIds))
            {
                criteria.CategoryIds = request.CategoryIds
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => int.TryParse(s.Trim(), out var id) ? id : -1)
                    .Where(id => id > 0)
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(request.AttributeKey) && !string.IsNullOrWhiteSpace(request.AttributeValue))
            {
                criteria.Attributes = new Dictionary<string, string>
                {
                    { request.AttributeKey, request.AttributeValue }
                };
            }

            var products = await _ProductService.SearchProductsAsync(criteria);
            return Ok(products.Select(ProductResponse.FromProduct).ToList());
        }
    }
}
