using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Interview.Web.Repositories.Interfaces;
using Sparcpoint.Models;

namespace Interview.Web.Controllers
{
    // EVAL
    [Route("api/v1/products")]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        
        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productRepository.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var createdProduct = await _productRepository.CreateProduct(product);
            return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.ProductID }, createdProduct);
        }
        
        [HttpPost]
        public async Task<IActionResult> AddProduct(Product product)
        {
            try
            {
                await _productRepository.AddProduct(product);
                return Ok(product);
            }
            catch(Exception ex)
            {
                return StatusCode(500, "An error occurred while adding the product. " + ex.Message);
            }
        }
        
        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts(string category, string name, string sku, string brand, string color)
        {
            try
            {
                var products = await _productRepository.SearchProducts(category, name, sku, brand, color);
                return Ok(products);
            }
            catch(Exception ex)
            {
                return StatusCode(500, "An error occurred while searching the products. " + ex.Message);
            }
        }
        
        // NOTE: Sample Action
        [HttpGet]
        public Task<IActionResult> GetAllProducts()
        {
            return Task.FromResult((IActionResult)Ok(new object[] { }));
        }
    }
}
