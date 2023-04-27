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
        [HttpGet]
        public Task<IActionResult> GetAllProducts()
        {
            return Task.FromResult((IActionResult)Ok(new object[] { }));
        }

        [HttpPost]
        public Task<IActionResult> AddNewProduct()
        {
            return Task.FromResult((IActionResult)Ok(new object[] { }));
        }

        [HttpPost]
        public Task<IActionResult> SearchProduct()
        {
            return Task.FromResult((IActionResult)Ok(new object[] { }));
        }

        [HttpPut]
        public Task<IActionResult> AddProductToInventory()
        {
            return Task.FromResult((IActionResult)Ok(new object[] { }));
        }

        [HttpDelete]
        public Task<IActionResult> RemoveProductFromInventory()
        {
            return Task.FromResult((IActionResult)Ok(new object[] { }));
        }

        [HttpGet]
        public Task<IActionResult> RetrieveProductCount()
        {
            return Task.FromResult((IActionResult)Ok(new object[] { }));
        }
    }
}
