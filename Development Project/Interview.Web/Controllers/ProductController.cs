using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Abstract.Services;
using Sparcpoint.DTOs;

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
            var products = await _productService.GetAllSync();

            return Ok(products);
        }

        /*
         * I assumed that the category already exists in their table
         */
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] ProductDTO product)
        {
            if (product is null) 
            {
                return BadRequest();
            }

            var result = await _productService.Create(product);

            return Ok(result);
        }

        [HttpPost("search")]
        public async Task<IActionResult> SearchProducts([FromBody] ProductDTO request)
        {
            if (request is null)
            {
                return BadRequest();
            }

            var productsSearched = await _productService.SearchAsync(request);

            return Ok(productsSearched);
        }
    }
}
