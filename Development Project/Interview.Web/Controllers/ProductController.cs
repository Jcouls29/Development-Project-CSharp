using Interview.Web.Contracts;
using Interview.Web.DTOs;
using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Domain;
using System;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [Route("api/v1/products")]
    public class ProductController : Controller
    {
        private readonly IProductAppService productAppService;

        public ProductController(IProductAppService productAppService)
        {
            this.productAppService = productAppService;
        }

        /** Create Product Endpoint
         * Example Request Body:
         * {
         *   "name": "Test Product",
         *   "description": "This is a test product.",
         *   "productImageUris": "http://example.com/image1.jpg,http://example.com/image2.jpg",
         *   "validSkus": "SKU123,SKU456",
         *   "attributes": [
         *     {"key": "Color", "value": "Red"},
         *     {"key": "Size", "value": "M"}
         *   ],
         *   "categoryIds": [1, 2]
         * }
         */
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto product)
        {
            int instanceId = 0;
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                instanceId = await productAppService.AddProduct(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            return CreatedAtAction(nameof(CreateProduct), new {InstanceId = instanceId});
        }

        /** Get Product Endpoint
         * Example Request: GET /api/v1/products?filter=Name:Test,Color:Azult,Largo:10CM
         */
        [HttpGet]
        public async Task<IActionResult> GetProducts(string filter)
        {
            try
            {
                if(string.IsNullOrEmpty(filter))
                {
                    return BadRequest("Filter query parameter is required.");
                }
                var result = await productAppService.GetProduct(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
