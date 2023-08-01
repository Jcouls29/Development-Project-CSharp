using Interview.Web.Data;
using Interview.Web.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [Route("api/v1/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IInventoryRepo _inventoryRepo;

        public ProductController(IInventoryRepo inventoryRepo)
        {
            _inventoryRepo = inventoryRepo;
        }

        [HttpGet]
        public Task<IActionResult> GetAllProducts()
        {
            return Task.FromResult((IActionResult)Ok(new object[] { }));
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductCreateDto productCreateDto)
        {
            try
            {
                var createdProduct = await _inventoryRepo.CreateProduct(productCreateDto);

                return CreatedAtRoute("ProductById", new { id = createdProduct.InstanceId }, createdProduct);
            }
            catch (Exception ex)
            {
                //HTTP status code 500 indicates that the server encountered an unexpected error while processing the request.
                Console.WriteLine($"--> Could not create product: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}", Name = "ProductById")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var product = await _inventoryRepo.GetProductById(id);

                if (product == null)
                    return NotFound();
                return Ok(product);
            }
            catch(Exception ex)
            {
                //HTTP status code 500 indicates that the server encountered an unexpected error while processing the request.
                Console.WriteLine($"--> Could not get product by Id: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
