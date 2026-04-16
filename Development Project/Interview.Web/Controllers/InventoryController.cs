using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Interview.Web.Controllers
{
    [Route("api/v1/inventory")]
    [ApiController]
    public class InventoryController : Controller
    {
        [HttpGet]
        public Task<IActionResult> GetAllProducts()
        {
            return Task.FromResult((IActionResult)Ok(new object[] { }));
        }
    }
}
