using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Sparcpoint.Abstract;
using Sparcpoint.Models.Requests;
using Sparcpoint.Models.Tables;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    //EVAL: should build in validation service to ensure that model validation is being done prior to entry
    //EVAL: need to add in error handling
    //EVAL: should add authorization
    [Route("api/v1/products")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _productService.GetProductsAsync();
            return Ok(products);
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<IActionResult> GetProduct(int id)
        {
            Product product = await _productService.GetProductAsync(id);
            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] Product request)
        {
            await _productService.AddProductAsync(request);
            return Ok();
        }

        [Route("query")]
        [HttpPost]
        public async Task<IActionResult> QueryProducts([FromBody] ProductRequest? request)
        {
            var products = await _productService.GetProductsAsync(request);
            return Ok(products);
        }
    }
}