using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Data;
using Sparcpoint.Dto;
using System;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [Route("api/v1/products")]
    public class ProductController(ProductsRepository productsRepository) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await productsRepository.GetAllProductsAsync();

            if (products is null || products.Count == 0)
            {
                return NoContent();
            }

            return Ok(products);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var product = await productsRepository.GetProductByIdAsync(id);

            if (product is null || product.InstanceId == 0)
            {
                return NoContent();
            }

            return Ok(product);
        }

        [HttpGet("Count")]
        public async Task<IActionResult> GetProductCount()
        {
            var count = await productsRepository.GetProductCountAsync();

            return Ok(count);
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] Product product)
        {
            var savedProduct = new Product();
            try
            {
                savedProduct = await productsRepository.AddProductAsync(product);

                return Ok(savedProduct);
            }
            catch (Exception e)
            {
                // This would not be appropriate in a production application.
                // Logging and a friendly answer would be better in production
                return Problem(e.ToString());
            }
        }


    }
}
