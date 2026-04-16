using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Application.DTOs;
using Sparcpoint.Application.Interfaces;
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

        // NOTE: Sample Action
        [HttpGet]
        public Task<IActionResult> GetAllProducts()
        {
            return Task.FromResult((IActionResult)Ok(new object[] { }));
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
        {
            var productId = await _productService.CreateProduct(request);

            return Ok(new { productId });
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] ProductSearchRequest request)
        {
            var results = await _productService.SearchProducts(request);
            return Ok(results);
        }
    }
}
