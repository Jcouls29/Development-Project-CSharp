using Interview.Web.Mappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{

    [ApiVersion("1.0")]
    [Route("api/v1/inventory")]
    public class InventoryController : Controller
    {

        private readonly IMediator _mediator;
        public InventoryController(IMediator mediator)
        {
            _mediator = mediator;

        }
        /// <summary>
        /// Adds new product(s) to the inventory
        /// </summary>
        /// <returns></returns>
        [Route("Products")]
        [HttpPost]

        public async Task<IActionResult> AddProductsToInventory(Dictionary<int, int> product_quantity)
        {
            if (ModelState.IsValid)
            {
                var newInventoryCommand = ProductRequestMapper.MapInventoryRequestToCommand(product_quantity);
                return Ok(await _mediator.Send(newInventoryCommand));
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
