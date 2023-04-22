using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Abstractions;
using System;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [Route("api/v1/inventory")]
    public class InventoryController : Controller
    {
        //EVAL: We should look to leverage client specific database configuration. This gives us performance assurance and flexability for BYOK
        //EVAL: Update to leverage transactions

        private readonly IInventoryOperator _inventoryOperator;

        public InventoryController(IInventoryOperator inventoryOperator)
        {
            _inventoryOperator = inventoryOperator;
        }

        // NOTE: GetAll Action
        [HttpPost]
        public Task<IActionResult> SearchInventory(string evaluator)
        {
            if (evaluator is null)
            {
                throw new ArgumentNullException(nameof(evaluator));
            }

            return Task.FromResult((IActionResult)Ok(new object[] { }));
        }

        // NOTE: Add Product Action
        [HttpPut]
        public Task<IActionResult> AddProductToInventory(string productId)
        {
            if (productId is null)
            {
                throw new ArgumentNullException(nameof(productId));
            }

            return Task.FromResult((IActionResult)Ok(new object[] { }));
        }

        // NOTE: Add Product Action
        [HttpDelete]
        public Task<IActionResult> RemoveProductFromInventory(string inventoryId)
        {
            if (inventoryId is null)
            {
                throw new ArgumentNullException(nameof(inventoryId));
            }

            return Task.FromResult((IActionResult)Ok(new object[] { }));
        }

        // NOTE: Add Product Action
        [HttpGet]
        public Task<IActionResult> GetProductCountById(string productId)
        {
            if (productId is null)
            {
                throw new ArgumentNullException(nameof(productId));
            }

            return Task.FromResult((IActionResult)Ok(new object[] { }));
        }
    }
}
