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
            await _productService.CreateProductAsync(req);
            return Ok();
        }

        [HttpPost("/search")]
        public async Task<IActionResult> SearchProducts(ProductSearchRequest req)
        {
            var results = _productService.SearchProducts(req);
            return Ok(results);
        }

        [HttpPost("/{productId}/attributes")]
        public async Task<IActionResult> AddAttributesToProduct(int productId, List<KeyValuePair<string, string>> attributes)
        {
            await _productService.AddAttributesToProduct(productId, attributes);
            return Ok();
        }

        [HttpPost("/{productId}/categories")]
        public async Task<IActionResult> AddProductToCategories(int productId, List<int> categories)
        {
            await _productService.AddProductToCategories(productId, categories);
            return Ok();
        }
    }
}
