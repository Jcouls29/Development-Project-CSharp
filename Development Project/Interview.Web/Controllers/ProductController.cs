using Interview.Web.Contracts;
using Interview.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [ApiController]
    [Route("api/v1/products")]
    public class ProductController : ControllerBase
    {
        private readonly IInventoryManagementService _inventoryManagementService;

        public ProductController(IInventoryManagementService inventoryManagementService)
        {
            _inventoryManagementService = inventoryManagementService;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ProductResponse>>> GetAllProducts()
        {
            var products = await _inventoryManagementService.SearchProductsAsync(new SearchProductsRequest());
            return Ok(products);
        }

        [HttpGet("{productId:int}")]
        public async Task<ActionResult<ProductResponse>> GetProduct(int productId)
        {
            var product = await _inventoryManagementService.GetProductAsync(productId);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<ProductResponse>> CreateProduct([FromBody] CreateProductRequest request)
        {
            var product = await _inventoryManagementService.CreateProductAsync(request);
            return CreatedAtAction(nameof(GetProduct), new { productId = product.ProductId }, product);
        }

        [HttpPut("{productId:int}")]
        public async Task<ActionResult<ProductResponse>> UpdateProduct(int productId, [FromBody] UpdateProductRequest request)
        {
            var product = await _inventoryManagementService.UpdateProductAsync(productId, request);
            return Ok(product);
        }


        [HttpPost("search")]
        public async Task<ActionResult<IReadOnlyList<ProductResponse>>> SearchProducts([FromBody] SearchProductsRequest request)
        {
            var products = await _inventoryManagementService.SearchProductsAsync(request);
            return Ok(products);
        }
    }
}
