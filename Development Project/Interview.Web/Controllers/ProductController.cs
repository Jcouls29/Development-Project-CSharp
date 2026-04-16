using Microsoft.AspNetCore.Mvc;
using Sparcpoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [ApiController]
    [Route("api/v1/products")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Product product)
        {
            // EVAL: Use of IActionResult to return correct HTTP status codes (201 Created)
            var id = await _productService.CreateAsync(product);
            return CreatedAtAction(nameof(Create), new { id }, product);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] int[] categoryIds,
            [FromQuery] string attrKey = null,
            [FromQuery] string attrValue = null)
        {
            // EVAL: Allow searching by multiple categories (api/products/search?categoryIds=1&categoryIds=2)
            var results = await _productService.SearchAsync(categoryIds, attrKey, attrValue);

            if (!results.Any())
                return NotFound("Cannot find products with the specified criteria.");

            return Ok(results);
        }

        // NOTE: Sample Action
        [HttpGet]
        public Task<IActionResult> GetAllProducts()
        {
            return Task.FromResult((IActionResult)Ok(new object[] { }));
        }
    }
}
