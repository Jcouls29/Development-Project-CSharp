using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Features.Products.Commands.Add;
using Sparcpoint.Features.Products.Queries.GetById;
using Sparcpoint.Features.Products.Queries.GetProducts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [Route("api/v1/products")]
    
    public class ProductController : Controller
    {
        private readonly IMediator mediator;

        public ProductController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetProductsAsync([FromQuery] ProductsGetQuery query)
        {
            var result = await mediator.Send(query ?? new ProductsGetQuery());
            return Ok(result);
        }

        [HttpGet("{id}", Name = "GetProductById")]
        public async Task<IActionResult> GetProductByIdAsync(int id)
        {
            var result = await mediator.Send(new ProductGetByIdQuery { Id = id });
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddProductAsync([FromBody] ProductAddCommand command)
        {
            if (command == null)
                return BadRequest("Product data is required.");
            try
            {
                var result = await mediator.Send(command);
                return CreatedAtRoute("GetProductById", new { id = result.InstanceId }, result);
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return StatusCode(500, $"An error occurred while adding the product: {ex.Message}");
            }
        }
    }
}
