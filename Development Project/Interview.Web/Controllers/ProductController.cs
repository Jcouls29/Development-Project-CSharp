using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Inventory.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    /// <summary>
    /// Manages product catalog operations: creating and searching products.
    /// </summary>
    // EVAL: [ApiController] gives us automatic model validation (400 on bad requests) and drops the
    // need for explicit [FromBody] on POST params. ControllerBase because there's no Razor views here.
    [ApiController]
    [Route("api/v1/products")]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _Products;

        public ProductController(IProductRepository products)
        {
            _Products = products ?? throw new ArgumentNullException(nameof(products));
        }

        /// <summary>
        /// Adds a new product with its metadata and category assignments.
        /// Products cannot be deleted once added (by design per requirements).
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(int), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> AddProduct([FromBody] CreateProductRequest request)
        {
            var productId = await _Products.AddAsync(request);
            // EVAL: returning 201 with the new ID - a full REST impl would return a Location header
            // pointing to GET /products/{id} but that's out of scope here
            return StatusCode(201, productId);
        }

        /// <summary>
        /// Searches products by any combination of name, metadata attributes, and categories.
        /// All supplied fields are combined with AND semantics.
        /// </summary>
        /// <param name="name">Optional partial name match (case-insensitive LIKE).</param>
        /// <param name="categoryIds">Optional category IDs - product must belong to at least one.</param>
        /// <param name="attributeKey">Optional metadata key to filter by.</param>
        /// <param name="attributeValue">Optional metadata value to filter by (paired with attributeKey).</param>
        /// <param name="page">Optional 1-based page number. Requires pageSize to activate pagination.</param>
        /// <param name="pageSize">Optional page size (max 200). Requires page to activate pagination.</param>
        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<Product>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> SearchProducts(
            [FromQuery] string name = null,
            [FromQuery] int[] categoryIds = null,
            [FromQuery] string attributeKey = null,
            [FromQuery] string attributeValue = null,
            [FromQuery] int? page = null,
            [FromQuery] int? pageSize = null)
        {
            var filter = new ProductSearchFilter
            {
                Name = name,
                CategoryIds = categoryIds?.Length > 0 ? categoryIds : null,
                Page = page,
                PageSize = pageSize
            };

            // EVAL: single key/value attribute filter via query string for simplicity - a proper
            // production API would take a POST body for richer multi-attribute searches
            if (!string.IsNullOrWhiteSpace(attributeKey) && !string.IsNullOrWhiteSpace(attributeValue))
            {
                filter.Attributes = new Dictionary<string, string>
                {
                    [attributeKey] = attributeValue
                };
            }

            var results = await _Products.SearchAsync(filter);
            return Ok(results);
        }
    }
}
