using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Models;
using Sparcpoint.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [Route("api/v1/products")]
    public class ProductController : Controller
    {
        private IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // NOTE: Sample Action
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetProducts();
            return Ok(products);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(CreateProductRequest req)
        {
            //if there is a validation step for skus it should be here
            var product = await _productService.CreateProductAsync(req);
            return Ok(product);
        }
    }
}
