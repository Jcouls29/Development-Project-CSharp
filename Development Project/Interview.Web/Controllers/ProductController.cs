using LamarCodeGeneration.Util;
using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Inventory.Handler.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Sparcpoint.InventoryService.Common.Extensions;
using Microsoft.AspNetCore.Http;
using Sparcpoint.Inventory.Core.Response;

namespace Interview.Web.Controllers
{
    [ApiController]
    [Route("api/v1/products")]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator.ThrowIfNull<IMediator>(nameof(mediator));
        }

        // NOTE: Sample Action
        [HttpGet]
        public Task<IActionResult> GetAllProducts()
        {
            return Task.FromResult((IActionResult)Ok(new object[] { }));
        }

        [HttpPost("~/api/v1/product")]
        public async Task<ActionResult<AddProductResponse>> CreateProductAsync([FromBody] AddProductCommand command)
        {
            if (command == null)
            {
                return BadRequest("Request was null");
            }

            try
            {
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
