using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Entities;
using Sparcpoint.Repositories;
using System;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [Route("api/v1/products")]
    public class ProductController : Controller
    {
        private readonly IProdcutRepository _productRepository;

        public ProductController(IProdcutRepository prodcutRepository)
        {
            _productRepository = prodcutRepository;
        }

        // NOTE: Sample Action
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            try
            {
                var products = await _productRepository.GetProducts();
                return Ok(products);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductForCreationDto productForCreation)
        {
            try
            {
                var createdProduct = await _productRepository.CreateProduct(productForCreation);

                return CreatedAtRoute("InstanceId",new { id = createdProduct.InstanceId },createdProduct);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


    }
}
