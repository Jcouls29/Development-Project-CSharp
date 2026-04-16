using Interview.Web.DTOs;
using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Abstract.Services;
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

        // add product
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto productDto)
        {
            if (productDto == null) return BadRequest("Invalid product.");

            try
            {
                return await _productService.AddProductAsync(productDto);
                
            }
            catch (Exception ex)

            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

    }
}
