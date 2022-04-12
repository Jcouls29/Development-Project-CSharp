using Interview.Web.Mappers;
using Interview.Web.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v1/products")]


    public class ProductController : Controller
    {
        // NOTE: Sample Action
        [HttpGet]
        public Task<IActionResult> GetAllProducts()
        private readonly IMediator _mediator;
        public ProductController(IMediator mediator)
        {
            return Task.FromResult((IActionResult)Ok(new object[] { }));
            _mediator = mediator;

        }

        /// <summary>
        /// Creates a New Product.
        /// </summary>

        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(ProductRequestDto newProduct)
        {
            if (ModelState.IsValid)
            {
                var newProductCommand = ProductRequestMapper.MapProductRequestToCommand(newProduct);
                return Ok(await _mediator.Send(newProductCommand));

            }
            else
            {
                return BadRequest();
            }
        }

    }
}