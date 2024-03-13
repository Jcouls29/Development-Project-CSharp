using Interview.Web.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Interview.Web.Models;

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
            var allProducts = await _productService.GetProducts();
            return Ok(allProducts);
        }

        public async Task<IActionResult> CreateProduct(Product productModel)
        {
            try
            {
                await _productService.CreateProduct(productModel);
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(500, "Error creating a new product");
            }
        }
    }
}
