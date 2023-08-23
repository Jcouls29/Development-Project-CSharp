using Interview.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    public class DomainController : Controller
    {
        // Todo: Add remove product from inventory
        [HttpGet]
        public Task<IActionResult> AddProductToInventory(ProductDTO product)
        {
            return Task.FromResult((IActionResult)Ok(new object[] { }));
        }

        // Todo: Add remove product from inventory
        [HttpGet]
        public Task<IActionResult> RemoveProductFromInventory()
        {
            return Task.FromResult((IActionResult)Ok(new object[] { }));
        }

        // Todo: add products
        [HttpPost]
        public Task<IActionResult> AddProduct(object productAttribute)
        {
            return Task.FromResult((IActionResult)Ok(productAttribute));
        }

        // Todo: catagorize items
        [HttpPost]
        public Task<IActionResult> DisplayByCategory(object productAttribute)
        {
            return Task.FromResult((IActionResult)Ok(productAttribute));
        }
        
        // Todo: seach metadata
        [HttpPost]
        public Task<IActionResult> SearchByMetadata(object productAttribute)
        {
            return Task.FromResult((IActionResult)Ok(productAttribute));
        }
    }
}
