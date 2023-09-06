using Interview.Web.Services;
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
		public readonly IProductSerivce ProductSerivce;

        public ProductController(IProductSerivce productSerivce) 
        {
            ProductSerivce = productSerivce ?? throw new ArgumentException($"{nameof(IProductSerivce)} DI cannot be null");
        }

        // NOTE: Sample Action
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await ProductSerivce.GetAllProducts();

            return new OkObjectResult(products);
        }
    }
}
