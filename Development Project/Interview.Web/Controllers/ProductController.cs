using Interview.Web.Core.DomainModel;
using Interview.Web.Core.Repositories;
using Interview.Web.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    public class ProductController : ControllerBase
    {
        internal readonly IProduct _product;
        public ProductController(IProduct product)
        {
            _product = product;
        }
        /// <summary>
        /// Geeting all products
        /// </summary>
        /// <returns></returns>
        // NOTE: Sample Action
        [HttpGet]
        [Route("api/v1/products")]
        public Task<IActionResult> GetAllProducts()
        {
            return Task.FromResult((IActionResult)Ok(_product.SearchProduct()));
        }

        /// <summary>
        /// AddProduct
        /// Adding a Product to Products table
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v1/products/add")]
        public Task<IActionResult> AddProduct([FromBody]ProdusctRequest products)
        {
            return Task.FromResult((IActionResult)Ok(_product.AddProduct(products)));
        }
    }
}
