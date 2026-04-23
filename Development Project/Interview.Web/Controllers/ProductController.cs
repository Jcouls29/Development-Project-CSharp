using Interview.Web.Models.Product;
using Interview.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        // POST: api/product
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
        {
            try
            {
                var productId = await _productService.CreateProductAsync(request);

                return Ok(new
                {
                    ProductId = productId,
                    Message = "Product created successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Creating product");
                return StatusCode(500, "Internal Server Error");

            }
        }

        // POST: api/product/search
        [HttpPost("search")]
        public async Task<IActionResult> SearchProducts([FromBody] ProductSearchRequest request)
        {
            try
            {
                var result = await _productService.SearchProductsAsync(request);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Searcing product");
                return StatusCode(500, "Internal Server Error");

            }
        }
    }
}