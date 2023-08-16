using Interview.Service.Models;
using Interview.Service.Products;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [Route("api/v1/products")]
    public class ProductController : Controller
    {
        private readonly IProductRepo _repo;

        public ProductController(IProductRepo repo)
        {
            // EVAL: Not currently expecting business logic outside of query building, if this changes then a business service layer would be introduced.
            _repo = repo;
        }

        [HttpPost]
        [Route("AddProducts")]
        public Task<IActionResult> AddProducts([FromBody] List<Product> products)
        {
            try
            {
                if (products.Count == 0)
                    return Task.FromResult((IActionResult)BadRequest("No products found in the list to add"));

                var result = _repo.AddProducts(products);

                // EVAL: Could wrap the added products in a response object if more info was needed by the response
                return Task.FromResult((IActionResult)Ok(result));
            }
            catch (Exception ex)
            {
                var message = $"An error occurred when attempting to save a list of products {ex.Message}";
                // EVAL: Log message to db (message, ex.StackTrace)
                return Task.FromResult((IActionResult)StatusCode(500, new { message }));
            }
        }

        [HttpPost]
        [Route("FilterProducts")]
        public Task<IActionResult> GetFilteredProducts([FromBody] ProductFilterParams filterParams)
        {
            try
            {
                var result = _repo.RetreiveProducts(filterParams);

                if (result.Count == 0)
                    return Task.FromResult((IActionResult)NotFound("No product results were found using the supplied filter values"));

                // EVAL: Could wrap the added products in a response object if more info was needed by the response
                return Task.FromResult((IActionResult)Ok(result));
            }
            catch (Exception ex)
            {
                var message = $"An error occurred when attempting to retrieve a filtered list of products {ex.Message}";
                // EVAL: Log message to db (message, ex.StackTrace)
                return Task.FromResult((IActionResult)StatusCode(500, new { message }));
            }
        }
    }
}
