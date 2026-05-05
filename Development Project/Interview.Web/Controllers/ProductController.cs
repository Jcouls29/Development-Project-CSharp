using Interview.Web.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Inventory;
using Sparcpoint.Inventory.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [Route("api/v1/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _Products;

        public ProductController(IProductRepository products)
        {
            _Products = products ?? throw new ArgumentNullException(nameof(products));
        }

        /// <summary>GET /api/v1/products?name=X&amp;categoryIds=1&amp;categoryIds=2</summary>
        [HttpGet]
        public async Task<IActionResult> Search(
            [FromQuery] string name,
            [FromQuery] int[] categoryIds)
        {
            var filter = new ProductSearchFilter
            {
                Name = name,
                CategoryIds = categoryIds?.Length > 0 ? categoryIds : null
            };

            return Ok(await _Products.SearchAsync(filter));
        }

        /// <summary>POST /api/v1/products/search — full-featured search including attribute filters</summary>
        [HttpPost("search")]
        public async Task<IActionResult> AdvancedSearch([FromBody] SearchProductsRequest request)
        {
            var filter = new ProductSearchFilter
            {
                Name = request?.Name,
                CategoryIds = request?.CategoryIds,
                Attributes = request?.Attributes
            };

            return Ok(await _Products.SearchAsync(filter));
        }

        /// <summary>GET /api/v1/products/{id}</summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _Products.GetByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        /// <summary>POST /api/v1/products — create a product; products can never be deleted per spec</summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Name))
                return BadRequest("Name is required.");

            var entry = new ProductEntry
            {
                Name = request.Name,
                Description = request.Description ?? string.Empty,
                ProductImageUris = request.ProductImageUris ?? Array.Empty<string>(),
                ValidSkus = request.ValidSkus ?? Array.Empty<string>(),
                Attributes = request.Attributes ?? new Dictionary<string, string>(),
                CategoryIds = request.CategoryIds ?? Array.Empty<int>()
            };

            int id = await _Products.AddAsync(entry);
            return CreatedAtAction(nameof(GetById), new { id }, new { InstanceId = id });
        }
    }
}
