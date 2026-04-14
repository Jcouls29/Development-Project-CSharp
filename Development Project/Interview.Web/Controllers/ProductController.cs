using Interview.Web.Contracts;
using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Inventory.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [ApiController]
    [Route("api/v1/products")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductController(IProductService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
        {
            if (request is null) return BadRequest();
            var id = await _service.AddProductAsync(request.ToModel(), cancellationToken);
            return Created($"/api/v1/products/{id}", new { id });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var product = await _service.GetProductAsync(id, cancellationToken);
            return product is null ? NotFound() : Ok(product);
        }

        [HttpPost("search")]
        public async Task<IActionResult> SearchAsync([FromBody] ProductSearchRequest request, CancellationToken cancellationToken)
        {
            var criteria = (request ?? new ProductSearchRequest()).ToCriteria();
            var results = await _service.SearchAsync(criteria, cancellationToken);
            return Ok(results);
        }
    }
}
