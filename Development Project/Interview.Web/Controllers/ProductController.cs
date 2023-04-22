using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Abstractions;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [Route("api/v1/products")]
    public class ProductController : Controller
    {
        private readonly IProductOperator _productOperator;

        public ProductController(IProductOperator productOperator)
        {
            _productOperator = productOperator;
        }

        // NOTE: Sample Action
        [HttpGet]
        public Task<IActionResult> GetAllProducts()
        {
            return Task.FromResult((IActionResult)Ok(new object[] { }));
        }

        // NOTE: Search Action - nullable query values
        [HttpGet]
        public Task<IActionResult> FindProductById()
        {
            return Task.FromResult((IActionResult)Ok(new object[] { }));
        }

        // NOTE: Search Action - nullable query values
        [HttpGet]
        public Task<IActionResult> CreateProduct()
        {
            return Task.FromResult((IActionResult)Ok(new object[] { }));
        }

        // NOTE: Search Action - nullable query values
        [HttpGet]
        public Task<IActionResult> UpdateProduct()
        {
            return Task.FromResult((IActionResult)Ok(new object[] { }));
        }
    }
}
