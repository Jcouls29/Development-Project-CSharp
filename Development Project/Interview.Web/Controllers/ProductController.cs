using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Inventory.Abstract;
using Sparcpoint.Inventory.Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [Route("api/v1/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _Products;

        public ProductController(IProductRepository products)
        {
            _Products = products;
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] AddProductRequest request)
        {
            var instanceId = await _Products.AddAsync(request);
            return CreatedAtAction(nameof(SearchProducts), new { }, new { InstanceId = instanceId });
        }

        // EVAL: Search is exposed as POST /search so complex filter criteria (attributes, categories)
        // can be sent in the request body rather than wrestling with deeply nested query strings
        [HttpPost("search")]
        public async Task<IActionResult> SearchProducts([FromBody] ProductSearchRequest request)
        {
            var products = await _Products.SearchAsync(request ?? new ProductSearchRequest());
            return Ok(products);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _Products.SearchAsync(new ProductSearchRequest());
            return Ok(products);
        }
    }
}
