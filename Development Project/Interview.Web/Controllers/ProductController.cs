using AutoMapper;
using Interview.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Inventory.Abstract;
using Sparcpoint.Inventory.Models;
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
        private readonly IMapper _mapper;

        public ProductController(IProductService productService, IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets all products added to the system.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var products = await _productService.GetProductsAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                // EVAL: inject a logger and add logging
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Adds a new product to the system.
        /// </summary>
        /// <param name="request">The details for the product to be added.</param>
        /// <returns>The created product and its ID.</returns>
        [Route("create-product")]
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
        {
            // EVAL: add model validation, return a BadRequest with validation errors if needed
            try
            {
                var product = _mapper.Map<Product>(request);
                await _productService.CreateProductAsync(product);

                return CreatedAtRoute("InstanceId", new { id = product.InstanceId }, product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
