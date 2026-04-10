using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asp.Versioning;
using Interview.Web.DTOs;
using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Inventory.Models;
using Sparcpoint.Inventory.Services;

namespace Interview.Web.Controllers.V2
{
    /// <summary>
    /// EVAL: V2 Product controller extends V1 with bulk create and paginated search.
    /// Old clients on V1 are unaffected — Open-Closed Principle in action.
    /// </summary>
    [ApiVersion(2.0)]
    [Route("api/v{version:apiVersion}/products")]
    [ApiController]
    public class ProductV2Controller : ControllerBase
    {
        private readonly IProductService _ProductService;

        public ProductV2Controller(IProductService productService)
        {
            _ProductService = productService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
        {
            var product = MapToProduct(request);
            var created = await _ProductService.CreateProductAsync(product);
            return CreatedAtAction(nameof(GetProduct), new { id = created.InstanceId }, ProductResponse.FromProduct(created));
        }

        /// <summary>
        /// EVAL: V2 bulk create — partial failure support.
        /// Individual item errors are caught and reported without aborting the batch.
        /// </summary>
        [HttpPost("bulk")]
        public async Task<IActionResult> CreateProductsBulk([FromBody] List<CreateProductRequest> requests)
        {
            if (requests == null || requests.Count == 0)
                return BadRequest(new { error = "At least one product is required." });

            var results = new List<ProductResponse>();
            var errors = new List<object>();

            for (int i = 0; i < requests.Count; i++)
            {
                try
                {
                    var product = MapToProduct(requests[i]);
                    var created = await _ProductService.CreateProductAsync(product);
                    results.Add(ProductResponse.FromProduct(created));
                }
                catch (ArgumentException ex)
                {
                    errors.Add(new { index = i, error = ex.Message });
                }
            }

            return Ok(new { created = results, errors });
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
        /// EVAL: V2 paginated search with totalCount/totalPages metadata.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> SearchProducts(
            [FromQuery] SearchProductsRequest request,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 1;
            if (pageSize > 100) pageSize = 100;

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

            var allProducts = (await _ProductService.SearchProductsAsync(criteria)).ToList();
            var totalCount = allProducts.Count;
            var pagedProducts = allProducts.Skip((page - 1) * pageSize).Take(pageSize);

            return Ok(new
            {
                data = pagedProducts.Select(ProductResponse.FromProduct),
                page,
                pageSize,
                totalCount,
                totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            });
        }

        private static Product MapToProduct(CreateProductRequest request)
        {
            return new Product
            {
                Name = request.Name,
                Description = request.Description,
                ProductImageUris = request.ProductImageUris != null ? string.Join(",", request.ProductImageUris) : string.Empty,
                ValidSkus = request.ValidSkus != null ? string.Join(",", request.ValidSkus) : string.Empty,
                Attributes = request.Attributes ?? new Dictionary<string, string>(),
                CategoryIds = request.CategoryIds ?? new List<int>()
            };
        }
    }
}
