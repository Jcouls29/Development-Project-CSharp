using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Interview.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductInventoryController : ControllerBase
    {

        Inventory iproductinventory;
        public ProductInventoryController(Inventory _iproductinventory)
        {
            iproductinventory = _iproductinventory;
        }
        // GET: api/<ProductInventoryController>
        [HttpGet]
        public IEnumerable<string> GetAllProducts()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ProductInventoryController>/5
        [Route("api/v1/ProductInventory/getproductscount")]
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ProductInventoryController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ProductInventoryController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ProductInventoryController>/5
        [Route("api/v1/ProductInventory/removeproduct")]
        [HttpDelete("{id}")]
        public Task<IActionResult> DeleteProduct(int id)
        {
            bool response = iproductinventory.RemoveProducts(id);
            return Task.FromResult((IActionResult)Ok(response));
        }
    }
}
