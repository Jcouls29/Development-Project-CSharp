using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Core.Persistence.Entity.Sparcpoint.Entities;
using Sparcpoint.Infrastructure.Services.Interfaces;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [Route("api/v1/products")]
    [ApiController]
    [Consumes("application/json")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllProductAsync();

            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Product id is required.");
            }

            var product = await _productService.GetProductByIdAsync(id);

            return Ok(product);
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            // EVAL: Check if model state is valid before proceeding.
            // Can use model validation attributes on the model to handle this
            // or use FluentValidation to handle this.

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool createdProduct = await _productService.CreateProductAsync(product);

            return createdProduct ? StatusCode(201) : BadRequest();
        }

        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> UpdateProduct([FromBody] Product product)
        {
            // EVAL: Check if model state is valid before proceeding.
            // Can use model validation attributes on the model to handle this
            // or use FluentValidation to handle this.

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedProduct = await _productService.UpdateProductAsync(product);

            return Ok(updatedProduct);
        }

        [HttpDelete("{id}")]
        [Route("Delete")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Product id is required.");
            }

            bool deletedProduct = await _productService.DeleteProductAsync(id);

            return !deletedProduct ? BadRequest() : Ok();
        }
    }
}