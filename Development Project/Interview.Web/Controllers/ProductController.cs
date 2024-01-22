using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Abstract;
using Sparcpoint.Models;
using System.Threading.Tasks;

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
        public Task<IActionResult> GetAllProducts()
        {
            return Task.FromResult((IActionResult)Ok(new object[] { }));
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<IActionResult> GetProduct(int id)
        {
            Product product = await _productService.GetProductAsync(id);
            return Ok(product);
        }

        //TODO: change input type to specific add product type
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] Product request)
        {
            await _productService.AddProductAsync(request);
            return Ok();
        }

        //TODO: consider changing route, change body request specific for query requests
        [Route("query")]
        [HttpPost]
        public async Task<IActionResult> QueryProducts([FromBody] Product request)
        {
            await _productService.GetProductsAsync();
            return Ok();
        }
    }
}
