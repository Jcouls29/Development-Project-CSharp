using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sparcpoint.Abstract.Services;
using Sparcpoint.Models.DTOs;
using Sparcpoint.Models.Entity;
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
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        //EVAL - 1. API to add products to the system with necessary details and predefined metadata
        [HttpPost]

        public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequestDto request)
        {
            try
            {
                int productId = await _productService.AddProductAsync(request);
                return StatusCode(StatusCodes.Status201Created, new { productId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the product.");
            }
        }

        //EVAL: 2. Get product details by product id
        [HttpGet("{productId}")]
        public async Task<IActionResult> GetProduct([FromRoute] int productId)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(productId);
                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching products");
                return StatusCode(500, "An error occurred while searching for products.");
            }
        }

        //EVAL - 3. API to search product details by category, metadata or genral details
        [HttpPost("Search")]
        public async Task<IActionResult> SearchProducts([FromBody] SearchProductRequestDto request)
        {
            try
            {
                var products = await _productService.SearchProductAsync(request);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching products");
                return StatusCode(500, "An error occurred while searching for products.");
            }
        }
    }
}
