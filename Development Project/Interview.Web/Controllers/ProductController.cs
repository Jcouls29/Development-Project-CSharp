using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Interview.Web.Models;
using Interview.Web.Repositories.Interfaces;

namespace Interview.Web.Controllers
{
    [Route("api/v1/products")]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet]
        public async Task<IActionResult> SearchProducts([FromQuery] string category, [FromQuery] string attrKey, [FromQuery] string attrValue)
        {
            var results = await _productRepository.SearchProductsAsync(category, attrKey, attrValue);
            return Ok(results);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Name))
                return BadRequest("Invalid product data.");

            var productId = await _productRepository.AddProductAsync(request);
            return CreatedAtAction(nameof(SearchProducts), new { id = productId }, new { ProductId = productId });
        }
    }
}
