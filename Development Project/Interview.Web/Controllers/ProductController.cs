using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Inventory.Abstract;
using Sparcpoint.Inventory.Models;
using System;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    // EVAL: Route is explicitly versioned (/api/v1/...) so future breaking changes can ship as v2
    // without disrupting existing clients (per the recommendation about extending the API without
    // impacting older customers). Per requirement #1, there is intentionally no DELETE endpoint —
    // products cannot be deleted from the system.
    [ApiController]
    [Route("api/v1/products")]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _Products;

        public ProductController(IProductRepository products)
        {
            _Products = products;
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] string name, [FromQuery] int[] categoryIds, [FromQuery] string attributes)
        {
            var criteria = new ProductSearchCriteria
            {
                NameContains = name,
                CategoryIds = categoryIds
            };

            if (!string.IsNullOrWhiteSpace(attributes))
            {
                try
                {
                    criteria.AttributeFilters = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.Generic.Dictionary<string, string>>(attributes);
                }
                catch
                {
                    return BadRequest("'attributes' must be a JSON object of key/value pairs.");
                }
            }

            var results = await _Products.SearchAsync(criteria);
            return Ok(results);
        }

        [HttpGet("{productId:int}")]
        public async Task<IActionResult> GetById(int productId)
        {
            var product = await _Products.GetByIdAsync(productId);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Name))
                return BadRequest("Name is required.");

            try
            {
                var productId = await _Products.CreateAsync(request);
                var created = await _Products.GetByIdAsync(productId);
                return CreatedAtAction(nameof(GetById), new { productId }, created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
