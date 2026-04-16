using Interview.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Sparcpoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [Route("api/v1/products")]
    public class ProductController : Controller
    {
        private readonly IProductService _ProductService;

        public ProductController(IProductService productService)
        {
            _ProductService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> SearchProducts([FromQuery] SearchProductsRequest request)
        {
            var criteria = new ProductSearchCriteria
            {
                NameContains = request?.NameContains,
                DescriptionContains = request?.DescriptionContains,
                SkuContains = request?.SkuContains
            };

            if (!string.IsNullOrWhiteSpace(request?.CategoryIds))
            {
                criteria.CategoryIds = request.CategoryIds
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => { int.TryParse(s.Trim(), out var id); return id; })
                    .Where(id => id > 0)
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(request?.Metadata))
            {
                criteria.Metadata = new Dictionary<string, string>();
                var pairs = request.Metadata.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var pair in pairs)
                {
                    var parts = pair.Split(':', 2);
                    if (parts.Length == 2 && !string.IsNullOrWhiteSpace(parts[0]))
                    {
                        criteria.Metadata[parts[0].Trim()] = parts[1].Trim();
                    }
                }
            }

            var products = (await _ProductService.SearchProductsAsync(criteria))
                .Select(ProductResponse.FromProduct)
                .ToList();

            return Ok(products);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
        {
            if (request == null)
                return BadRequest("Request body is required.");

            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest("Product name is required.");

            if (request.Name.Length > 256)
                return BadRequest("Product name cannot exceed 256 characters.");
            if (request.Description?.Length > 256)
                return BadRequest("Product description cannot exceed 256 characters.");

            if (request.Metadata != null)
            {
                foreach (var kvp in request.Metadata)
                {
                    if (kvp.Key.Length > 64)
                        return BadRequest($"Metadata key '{kvp.Key}' cannot exceed 64 characters.");
                    if (kvp.Value?.Length > 512)
                        return BadRequest($"Metadata value for key '{kvp.Key}' cannot exceed 512 characters.");
                }
            }

            var product = MapToProduct(request);
            var created = await _ProductService.CreateProductAsync(product);

            return CreatedAtAction(nameof(GetProduct), new { id = created.InstanceId }, ProductResponse.FromProduct(created));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _ProductService.GetProductByIdAsync(id);

            if (product == null)
                return NotFound();

            return Ok(ProductResponse.FromProduct(product));
        }

        private static Product MapToProduct(CreateProductRequest request)
        {
            return new Product
            {
                Name = request.Name,
                Description = request.Description ?? string.Empty,
                ProductImageUris = request.ProductImageUris ?? string.Empty,
                ValidSkus = request.ValidSkus ?? string.Empty,
                Metadata = request.Metadata?.Select(kvp => new ProductMetadata
                {
                    Key = kvp.Key,
                    Value = kvp.Value
                }).ToList() ?? new List<ProductMetadata>(),
                CategoryIds = request.CategoryIds ?? new List<int>()
            };
        }
    }
}
