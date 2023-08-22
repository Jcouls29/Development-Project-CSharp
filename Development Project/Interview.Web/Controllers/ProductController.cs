using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Interview.Web.Models;

namespace Interview.Web.Controllers
{
    [Route("api/v1/products")]
    public class ProductController : Controller
    {
        private readonly IProductDB _ProductDB;
        // NOTE: Sample Action
        [HttpGet]
        public Task<IActionResult> GetAllProducts()
        {
            //return await _ProductDB.GetAllProducts(); // Not yet implemented
            return Task.FromResult((IActionResult)Ok(new object[] { }));
        }

        [HttpPut]
        public async Task<IActionResult> PutProduct(Product product)
        {
            if (product == null)
            {
                return BadRequest();
            }

            await _ProductDB.AddProductAsync(product);

            return NoContent();
        }
    }
}
