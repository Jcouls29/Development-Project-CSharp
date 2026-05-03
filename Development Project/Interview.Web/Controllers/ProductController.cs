using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Abstract;
using Sparcpoint.DTO;
using Sparcpoint.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    //EVAL: The ProductController class is an API controller that handles HTTP requests related to products.
    //It is decorated with the [ApiController] attribute, which indicates that it is an API controller
    //and enables features such as automatic model validation and binding.

    //EVAL: The [Route("api/v1/products")] attribute specifies the base route for all actions in this controller,
    //meaning that all endpoints defined in this controller will be prefixed with "api/v1/products".
    //The controller inherits from ControllerBase, which provides basic functionality for handling HTTP requests and responses.
    //without the need for views, making it suitable for building APIs.
    [ApiController]
    [Route("api/v1/products")]

    //EVAL: The constructor of the ProductController class takes two parameters: IProductRepository and IInventoryRespository.
    //These parameters are used to inject dependencies into the controller, allowing it to interact with the product repository and inventory repository.
    //The constructor initializes the private readonly fields _productsRepo and _inventoryRepo with the injected dependencies,
    //enabling the controller to use these repositories in its actions to perform operations related to products and inventory.
    public class ProductController(IProductRepository productsRepo) : ControllerBase
    {
        // NOTE: Sample Action
        private readonly IProductRepository _productsRepo = productsRepo;
        //GET api/v1/products
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productsRepo.GetAllProducts();

            //EVAL: If the repository returned null or an empty collection, return 204 No Content.
            // Otherwise return 200 OK with the product collection.
            if (products == null || !products.Any()) 
                return NoContent();
            
            return Ok(products);
        }

        // POST api/v1/products
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
        {
            if (request == null)
                return BadRequest("Request body is required.");

            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest("Product name is required.");

            int newId = await _productsRepo.CreateProductAsync(request);

            return Ok(newId);
        }
        //GET api/v1/products/search?searchCriteria=example
        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts([FromQuery] SearchProductRequest request) 
        {       
            var products = await _productsRepo.SearchProductAsync(request);
            //EVAL: If the repository returned null or an empty collection, return 204 No Content.
            // Otherwise return 200 OK with the product collection.
            if (products == null || !products.Any())
                return NoContent();

            return Ok(products);
        }
    }
}
