using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Models.Request.Product;
using Sparcpoint.Models.Response.Product;
using Sparcpoint.Services.Interfaces;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/v1/products")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(GetProductResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllProducts()
        {
            //We don't need to have GetProductRequest, but I follow Request, Response models so that it is consistent.
            var products = await _productService.GetAllProducts(new GetProductRequest());
            return Ok(products);
        }

        [HttpGet("GetProductById")]
        [ProducesResponseType(typeof(GetProductResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProductById([FromQuery] int productId)
        {
            var request = new GetProductRequestById(productId);
            var products = await _productService.GetProductById(request);
            return Ok(products);
        }

        [HttpPost("InsertProduct")]
        [ProducesResponseType(typeof(InsertProductResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Insert(InsertProductRequest insertProductRequest)
        {
            // Can Insert one or multiple products at same time
            var response = await _productService.InsertProduct(insertProductRequest);
            return Ok(response);
        }

        [HttpPost("UpdateProduct")]
        [ProducesResponseType(typeof(UpdateProductResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateDongle(UpdateProductRequest updateProductRequest)
        {
            var response = await _productService.UpdateProduct(updateProductRequest);
            return Ok(response);
        }
    }
}