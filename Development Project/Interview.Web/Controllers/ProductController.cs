using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Application.Abstract;
using Sparcpoint.Models.Products;
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
            _productService = productService;
        }

        /*EVAL : Different routes can be created for different client products,
         different product models can be passed so the client consumers know what needs to be send 
         like color,sku etc., which can be mapped to metadata on the application layer using business rules
         without affecting other clients.
        */

        // EVAL : Consumer represents different app within the same client

        [HttpPost]
        [Route("create-product/{consumer-id}")]
        public async Task<IActionResult> CreateProduct([FromBody] BaseProduct product,
            [FromRoute(Name = "consumer-id")] string consumerId)
        {
            try
            {
                //EVAL : We can also use the subcription key 
                //as an alternate approach to identify the consumer id from route
                return Ok(await _productService.CreateBaseProductServiceAsync(product, consumerId));
            }
            catch (Exception)
            {
                //EVAL : A middleware can be used here to generate appropriate error codes
                // and responses in real-world scenarios
                return StatusCode(500);
            }

        }

        [HttpGet]
        [Route("search-products/{consumer-id}")]
        public async Task<IActionResult> GetAllProducts([FromBody] BaseProduct searchProduct, 
            [FromRoute(Name = "consumer-id")] string consumerId)
        {
            try
            {
                return Ok(await _productService.GetBaseProductsServiceAsync(searchProduct, consumerId));
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        
    }
}
