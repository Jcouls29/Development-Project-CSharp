using Interview.Service.Models;
using Interview.Service.Inventory;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    public class InventoryController : Controller
    {
        private readonly IInventoryRepo _repo;

        public InventoryController(IInventoryRepo repo)
        {
            // EVAL: Not currently expecting business logic outside of query building, if this changes then a business service layer would be introduced.
            _repo = repo;
        }

        [HttpPost]
        [Route("InventoryCount")]
        public Task<IActionResult> GetInventoryCount([FromBody] ProductFilterParams parms)
        {
            try
            {
                var count = _repo.GetInventoryCount(parms);

                // EVAL: Could wrap the added products in a response object if more info was needed by the response
                return Task.FromResult((IActionResult)Ok(count));
            }
            catch (Exception ex)
            {
                var message = $"An error occurred when attempting to inventory count {ex.Message}";
                // EVAL: Log message to db (message, ex.StackTrace)
                // EVAL: Logging and 500 return could be thrown here and handled in an Exception Middleware component, which could be injected in Startup
                return Task.FromResult((IActionResult)StatusCode(500, new { message }));
            }
        }

        [HttpPost]
        [Route("AddInventory")]
        public Task<IActionResult> AddInventory([FromBody] List<InventoryTransaction> transactions)
        {
            try
            {
                if (transactions.Count == 0)
                    return Task.FromResult((IActionResult)BadRequest("There were no products to add to inventory."));

                var result = _repo.AddInventory(transactions);

                // EVAL: Could wrap the added products in a response object if more info was needed by the response
                return Task.FromResult((IActionResult)Ok(result));
            }
            catch (Exception ex)
            {
                var message = $"An error occurred when attempting to add inventory {ex.Message}";
                // EVAL: Log message to db (message, ex.StackTrace)
                // EVAL: Logging and 500 return could be thrown here and handled in an Exception Middleware component, which could be injected in Startup
                return Task.FromResult((IActionResult)StatusCode(500, new { message }));
            }
        }

        [HttpPost]
        [Route("DeleteInventory")]
        public Task<IActionResult> DeleteProductInventory(List<int> productIds)
        {
            try
            {
                // EVAL: May need to return something from delete action that can be used to verify that delete occurred on items
                _repo.DeleteInventory(productIds);

                return Task.FromResult((IActionResult)Ok());
            }
            catch (Exception ex)
            {
                var message = $"An error occurred when attempting to delete inventory {ex.Message}";
                // EVAL: Log message to db (message, ex.StackTrace)
                // EVAL: Logging and 500 return could be thrown here and handled in an Exception Middleware component, which could be injected in Startup
                return Task.FromResult((IActionResult)StatusCode(500, new { message }));
            }
        }
    }
}
