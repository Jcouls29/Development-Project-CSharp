using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Inventory.Interfaces;
using Sparcpoint.Inventory.Models;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [ApiController]
    [Route("api/v1/products")]
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
            var product = await _ProductService.CreateProductAsync(request);
            return CreatedAtAction(nameof(GetProduct), new { id = product.InstanceId }, product);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _ProductService.GetProductAsync(id);
            if (product == null)
                return NotFound(new ProblemDetails { Title = "Not Found", Detail = $"Product with ID {id} not found" });
            return Ok(product);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 25)
        {
            var request = new ProductSearchRequest { Page = page, PageSize = pageSize };
            var result = await _ProductService.SearchProductsAsync(request);
            return Ok(result);
        }

        [HttpPost("search")]
        public async Task<IActionResult> SearchProducts([FromBody] ProductSearchRequest request)
        {
            var result = await _ProductService.SearchProductsAsync(request);
            return Ok(result);
        }
    }
}
