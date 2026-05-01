using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Core.Abstract;
using Sparcpoint.Core.Models;

namespace Interview.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _repo;

        public ProductsController(IProductRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductCreateRequest request)
        {
            if (request is null)
                return BadRequest("Request body is required.");

            // Basic server-side validation (expand in service layer as needed)
            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest("Name is required.");

            var productId = await _repo.AddProductAsync(request);

            // Return 201 Created with location header
            var uri = $"/api/products/{productId}";
            return Created(uri, new { Id = productId });
        }

        /// <summary>
        /// Lightweight, URL-query searchable endpoint for common searches.
        /// Supports name, description, single sku, categoryId (repeatable), paging and simple ordering.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] string name = null,
            [FromQuery] string description = null,
            [FromQuery] string sku = null,
            [FromQuery(Name = "categoryId")] int[] categoryIds = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            [FromQuery] string orderBy = null,
            [FromQuery] bool orderDesc = false)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 50;
            if (pageSize > 500) pageSize = 500; // defensive cap

            var criteria = new ProductSearchCriteria
            {
                Name = name,
                Description = description,
                Skus = string.IsNullOrWhiteSpace(sku) ? null : new[] { sku },
                CategoryIds = categoryIds,
                Page = page,
                PageSize = pageSize,
                OrderBy = orderBy,
                OrderDescending = orderDesc
            };

            var results = await _repo.SearchProductsAsync(criteria) ?? new System.Collections.Generic.List<ProductSearchResult>();

            var response = new ProductSearchResponse
            {
                Items = results.ToList(),
                TotalCount = results.Count, // EVAL: replace with repo-provided total if available
                Page = page,
                PageSize = pageSize
            };

            return Ok(response);
        }

        /// <summary>
        /// Complex search using ProductSearchCriteria in the request body.
        /// Use this for metadata filters, multiple SKUs, match modes, etc.
        /// </summary>
        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] ProductSearchCriteria criteria)
        {
            if (criteria is null)
                return BadRequest("Request body is required.");

            if (criteria.Page <= 0) criteria.Page = 1;
            if (criteria.PageSize <= 0) criteria.PageSize = 50;
            if (criteria.PageSize > 500) criteria.PageSize = 500; // server-side cap

            var results = await _repo.SearchProductsAsync(criteria) ?? new System.Collections.Generic.List<ProductSearchResult>();

            var response = new ProductSearchResponse
            {
                Items = results.ToList(),
                TotalCount = results.Count, // EVAL: replace with repo-provided total if available
                Page = criteria.Page,
                PageSize = criteria.PageSize
            };

            return Ok(response);
        }
    }
}
