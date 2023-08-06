using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Http;
using System.Threading.Tasks;
using Inventory.BusinessServices;
using Inventory.BusinessServices.Services;

namespace Interview.Web.Controllers
{
    public class ProductController : ApiController
    {
        private IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IHttpActionResult> AddProductAsync([System.Web.Http.FromBody] Product product)
        {
            var response = await _productService.AddProductAsync(product);
            if (response.IsSuccessStatusCode)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest();
            }
        }

        [System.Web.Http.HttpPost]
        public async Task<IHttpActionResult> AddTransactionAsync( InventoryTransaction inventoryTransaction)
        {
            var response = await _productService.AddTransactionAsync(inventoryTransaction);
            if (response.IsSuccessStatusCode)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpDelete]
        public IHttpActionResult DeleteTransactionAsync(int transactionId)
        {
            var response =  _productService.DeleteTransactionAsync(transactionId);
            return Ok(response);
        }
    }
}
