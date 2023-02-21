using Interview.Entities;
using Interview.Services.Interfaces;
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
        private readonly IProductService _productService;
        ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // NOTE: Sample Action
        [HttpGet]
        public Task<IActionResult> GetAllProducts()
        {
            //added exception handling
            try
            {
                return Task.FromResult((IActionResult)Ok(new object[] { }));
            }
            catch(Exception ex)
            {
                //Add logging of the detailed exception
                //some logger implementation

                //return general error for the calling client
                return Task.FromResult((IActionResult)Problem("An error occurred"));
            }
        }

        public Task<IActionResult> AddProduct(Product product)
        {
            try
            {
                return Task.FromResult((IActionResult)Ok(_productService.Add(product)));
            }
            catch (Exception ex)
            {
                //Add logging of the detailed exception
                //some logger implementation

                //return general error for the calling client
                return Task.FromResult((IActionResult)Problem("An error occurred"));
            }
        }
    }
}
