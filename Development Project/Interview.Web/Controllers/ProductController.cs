using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sparcpoint.Inventory.Models;
using Sparcpoint.Inventory.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [Route("api/v1/products")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        // NOTE: Sample Action
        [HttpGet]
        public Task<IActionResult> GetAllProducts()
        {
            return Task.FromResult((IActionResult)Ok(new object[] { }));
        }

        [HttpPost("AddProduct")]
        public async Task<HttpResponseMessage> AddProduct([FromBody] Product product)
        {
            try
            {
                await _productService.AddProduct(product);
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.Message);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        //EVAL: SearchProducts not functional as usp_SearchProduct procedure has not been created
        [HttpGet("Search")]
        public Task<IActionResult> SearchProducts()
        {
            return Task.FromResult((IActionResult)Ok(new object[] { }));
        }
    }
}
