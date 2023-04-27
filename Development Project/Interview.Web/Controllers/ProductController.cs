using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sparcpoint.Implementations;
using System.Net.Http;
using Sparcpoint.Models;

namespace Interview.Web.Controllers
{
    [Route("api/v1/products")]
    public class ProductController : Controller
    {
        private readonly ProductService _productService;

        ProductController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public Task<HttpResponseMessage> GetAllProducts()
        {
            return Task.FromResult(_productService.GetProducts());
        }

        [HttpPost]
        public Task<IActionResult> AddNewProduct([FromBody] string jsonBody)
        {
            return Task.FromResult<IActionResult>(_productService.AddProduct(jsonBody));
        }

        [HttpPost]
        public Task<IActionResult> SearchProduct([FromBody] string jsonBody)
        {
            return Task.FromResult<IActionResult>(_productService.SearchProduct(jsonBody));
        }

        [HttpPut]
        public Task<IActionResult> AddProductToInventory()
        {
            return Task.FromResult<IActionResult>(_productService.AddInventory());
        }

        [HttpDelete]
        public Task<IActionResult> RemoveProductFromInventory()
        {
            return Task.FromResult<IActionResult>(_productService.RemoveInventory());
        }

        [HttpGet]
        public Task<IActionResult> RetrieveProductCount()
        {
            return Task.FromResult<IActionResult>(_productService.GetProductCount());
        }
    }
}
