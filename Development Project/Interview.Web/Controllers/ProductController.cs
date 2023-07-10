using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [Route("api/v1/products")]
    public class ProductController : Controller
    {
        // NOTE: Sample Action
        [HttpGet]
        public Task<IActionResult> GetAllProducts()
        {
            return Task.FromResult((IActionResult)Ok(new object[] { }));
        }

        // GET: api/v1/products/{productId}
        [HttpGet("{productId}")]
        public Task<IActionResult> GetProductById(Guid productId)
        {
            // Implement logic to retrieve a product by ID
            // ...

            return Task.FromResult((IActionResult)Ok(new
            {
                productId,
                name = "Sample Product",
                price = 10.99
            }));
        }

        //TODO: Implement the UnitOfWork and IProductRepository calls to add products, remove products, update products, etc.
        //TODO: Create the CategoryController to implement the UnitOfWork and ICategoryRepository calls to add, remove, update categories and more.

        // // POST: api/v1/products
        // // The Product is a Dto
        // [HttpPost]
        // public Task<IActionResult> AddProduct([FromBody] Product product)
        // {
        //     // Implement logic to add a product
        //     // ...

        //     return Task.FromResult((IActionResult)Ok(new
        //     {
        //         message = "Product added successfully",
        //         productId = Guid.NewGuid()
        //     }));
        // }

        // // POST: api/v1/products/search
        // [HttpPost("search")]
        // public Task<IActionResult> SearchProduct([FromBody] ProductSearchCriteria searchCriteria)
        // {
        //     // Implement logic to search for products
        //     // ...

        //     return Task.FromResult((IActionResult)Ok(new List<object>()));
        // }

        // // PUT: api/v1/products/{productId}/inventory
        // // The InventoryUpdate is a Dto
        // [HttpPut("{productId}/inventory")]
        // public Task<IActionResult> AddToInventory(Guid productId, [FromBody] InventoryUpdate update)
        // {
        //     // Implement logic to add products to inventory
        //     // ...

        //     return Task.FromResult((IActionResult)Ok(new
        //     {
        //         message = "Inventory updated successfully"
        //     }));
        // }

        // // DELETE: api/v1/products/{productId}/inventory
        // // The InventoryUpdate is a Dto
        // [HttpDelete("{productId}/inventory")]
        // public Task<IActionResult> RemoveFromInventory(Guid productId, [FromBody] InventoryUpdate update)
        // {
        //     // Implement logic to remove products from inventory
        //     // ...

        //     return Task.FromResult((IActionResult)Ok(new
        //     {
        //         message = "Inventory updated successfully"
        //     }));
        // }
    }
}
