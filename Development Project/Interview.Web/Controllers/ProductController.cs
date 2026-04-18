using Microsoft.AspNetCore.Mvc;
using Interview.Web.Contracts.Products;
using Interview.Web.Services.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [Route("api/v1/products")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        }

        // NOTE: Sample Action
        [HttpGet]
        public Task<IActionResult> GetAllProducts()
        {
            return Task.FromResult((IActionResult)Ok(new object[] { }));
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] CreateProductRequest request)
        {
            try
            {
                int productId = await _productService.AddProductAsync(request);
                return Created($"/api/v1/products/{productId}", new CreateProductResponse
                {
                    ProductId = productId
                });
            }
            catch (ProductValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts([FromQuery] string q, [FromQuery] string metadataKey, [FromQuery] string metadataValue, [FromQuery] string categoryIds)
        {
            try
            {
                var products = await _productService.SearchProductsAsync(q, metadataKey, metadataValue, categoryIds);
                return Ok(new SearchProductsResponse
                {
                    Products = products.ToList()
                });
            }
            catch (ProductValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{productId:int}/inventory/add")]
        public async Task<IActionResult> AddInventory(int productId, [FromBody] AdjustInventoryRequest request)
        {
            try
            {
                var result = await _productService.AddInventoryAsync(productId, request);
                return Ok(result);
            }
            catch (ProductValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{productId:int}/inventory/remove")]
        public async Task<IActionResult> RemoveInventory(int productId, [FromBody] AdjustInventoryRequest request)
        {
            try
            {
                var result = await _productService.RemoveInventoryAsync(productId, request);
                return Ok(result);
            }
            catch (ProductValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{productId:int}/inventory/count")]
        public async Task<IActionResult> GetInventoryCount(int productId)
        {
            try
            {
                decimal quantity = await _productService.GetInventoryCountAsync(productId);
                return Ok(new ProductInventoryCountResponse
                {
                    ProductId = productId,
                    Quantity = quantity
                });
            }
            catch (ProductValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
