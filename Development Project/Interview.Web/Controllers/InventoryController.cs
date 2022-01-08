using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [Route("api/v1/inventory")]
    public class InventoryController : Controller
    {
        IInventoryService _inventoryService;
        IValidationService _validationService;

        public InventoryController(IInventoryService inventoryService, IValidationService validationService)
        {
            _inventoryService = inventoryService;
            _validationService = validationService;
        }

        /// <summary>
        /// Gets all inventory transactions
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllInventoryTransactions()
        {
            var result = await _inventoryService.GetAllInventoryTransactions();
            return Ok(result);
        }

        /// <summary>
        /// Gets inventory for a supplied product
        /// </summary>
        [HttpGet("{productId}/quantity")]
        public async Task<IActionResult> GetInventoryForProduct(int productId)
        {

            if (productId == null)
            {
                return BadRequest("A product id must be provided");
            }

            var quantity = await  _inventoryService.GetInventoryForProduct(productId);
            return Ok(quantity);
        }

        /// <summary>
        /// Update inventory for a specific product
        /// </summary>
        [HttpPut("{productId}")]
        public async Task<IActionResult> UpdateProductInventory(int productId, int newInventoryCount)
        {
            if (productId == null)
            {
                return BadRequest("A product id must be provided");
            }

            var isValidQuantity = _validationService.QuantityIsValid(newInventoryCount);

            if (!isValidQuantity.IsValid)
            {
                return BadRequest($"{isValidQuantity.InvalidMessage}");
            }

            await _inventoryService.UpdateProductInventory(productId, newInventoryCount);
            return Ok();
        }

        /// <summary>
        /// Removes the indicated inventory transaction
        /// </summary>
        [HttpDelete("{transactionId}")]
        public async Task<IActionResult> RollbackInventoryUpdate(int transactionId)
        {
            if (transactionId == null)
            {
                return BadRequest("A transaction id must be provided");
            }

            await _inventoryService.RollbackInventoryUpdate(transactionId);
            return Ok();
        }
    }
}
