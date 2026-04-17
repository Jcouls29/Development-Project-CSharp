using System.Threading;
using System.Threading.Tasks;
using Interview.Web.Models;
using Interview.Web.Models.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Inventory.Repositories;

namespace Interview.Web.Controllers
{
    /// <summary>
    /// EVAL: Thin controller — only maps requests to domain and delegates to repository.
    /// All substantive logic lives in Sparcpoint.Inventory abstractions.
    /// </summary>
    [ApiController]
    [Route("api/v1/products")]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _products;

        public ProductController(IProductRepository products)
        {
            _products = products;
        }

        /// <summary>Creates a product. Returns 201 + the resource location.</summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddProduct([FromBody] AddProductRequest request, CancellationToken ct)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var id = await _products.AddAsync(request.ToDomain(), ct);
            return CreatedAtAction(nameof(GetProduct), new { id }, new { instanceId = id });
        }

        /// <summary>Gets a product by its InstanceId.</summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProduct(int id, CancellationToken ct)
        {
            var product = await _products.GetByIdAsync(id, ct);
            return product == null ? NotFound() : (IActionResult)Ok(product);
        }

        /// <summary>
        /// Search by metadata, category and/or partial name. Uses POST for the
        /// richness of body; GET + query-string would be limited for complex metadata.
        /// </summary>
        [HttpPost("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchProducts([FromBody] SearchProductsRequest request, CancellationToken ct)
        {
            var results = await _products.SearchAsync(request.ToDomain(), ct);
            return Ok(results);
        }
    }
}
