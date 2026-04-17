using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Abstract.Services;
using Sparcpoint.Models;
using Sparcpoint.Request;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [Route("api/v1/products")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        // NOTE: Sample Action
        [HttpGet]
        public Task<IActionResult> GetAllProducts()
        {
            return Task.FromResult((IActionResult)Ok(new object[] { }));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
        {
            var product = await _productService.CreateAsync(request);
            return CreatedAtAction(nameof(GetProduct), new { id = product });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound(new ProblemDetails { Title = "Not Found", Detail = $"Product {id} not found" });
            return Ok(product);
        }

        [HttpPost("search")]
        public async Task<IActionResult> SearchProducts([FromBody] ProductSearchRequest request)
        {           
            var result = await _productService.SearchAsync(request);
            return Ok(result);
        }
    }
}
