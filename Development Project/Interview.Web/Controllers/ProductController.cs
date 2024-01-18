using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Core.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [Route("api/v1/products")]
    public class ProductController : Controller
    {
        private readonly IProductManagementService _productManagementService;

        /// <summary>
        /// The constructor for the ProductController is used to inject the IProductManagementService
        /// </summary>
        /// <param name="productManagementService"></param>
        public ProductController(IProductManagementService productManagementService)
        {
            _productManagementService = productManagementService;
        }

        /// <summary>
        /// This method is used to return all products
        /// </summary>
        /// <remarks>All crud operations are handled by the productManagementService</remarks>
        [HttpGet]
        public async Task<IActionResult> Products()
        {

            try
            {
                var products = await _productManagementService.GetAllProductsAsync();

                // Eval : Return 204 No Content if there are no products
                if (!products.Any())
                {
                    return NoContent();
                }

                return Ok(products);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error, please try back or contact customer service for immediate assistance.");
            }
        }

        /// <summary>
        /// Get a single product and its attributes by name
        /// </summary>
        /// <param name="name"></param>
        [HttpGet("{name}")]
        public async Task<IActionResult> GetProduct(string name)
        {
            // Eval:Error handling
            try
            {
                var result = await _productManagementService.GetProductAsync(name);

                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Error, please try back or contact customer service").
            }
   
        }

        /// <summary>
        /// Add a product to the database
        /// </summary>
        /// <param name="product"></param>
        [HttpPost]
        public async Task<IActionResult> AddProductAsync([FromBody] Product product)
        {
            // EVAL: Add product and return 201 Created
            await _productManagementService.AddProductAsync(product);
            return CreatedAtAction(nameof(AddProductAsync), new { product.Name }, product);
        }

        /// <summary>
        /// Search for products by name, description, category or metadata
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts([FromQuery] ProductSearchCriteria criteria)
        {
            IEnumerable<Product> products = await _productManagementService.SearchProductsAsync(criteria);
            if (!products.Any()) return NoContent();
            return Ok(products);
        }
    }
}
