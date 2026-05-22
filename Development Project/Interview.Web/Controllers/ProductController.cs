using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Interview.Web.Models;
using Interview.Web.Services;

namespace Interview.Web.Controllers
{
    [Route("api/v1/products")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var result = await _productService.GetAllProducts();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] ProductCreateDto dto)
        {
            if (dto == null)
                return BadRequest();

            var product = await _productService.AddProduct(dto);

            return CreatedAtAction(
                nameof(GetAllProducts),
                new { id = product.Id },
                new
                {
                    product.Id,
                    product.Name,
                });
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(
            string? query,
            string? category,
            string? metaKey,
            string? metaValue)
        {
            var result = await _productService.Search(query, category, metaKey, metaValue);
            return Ok(result);
        }
    }
}
