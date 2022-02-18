using Interview.Data.Model;
using Interview.Data.Repository;
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
        private readonly IRepository<Product> _repository;

        public ProductController(IRepository<Product> repository)
        {
            this._repository = repository;
        }

        /// <summary>
        /// List All Products API
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public Task<IActionResult> GetAllProducts()
        {
            var products = this._repository.GetAll();
            return Task.FromResult((IActionResult)Ok(products));
        }

        /// <summary>
        /// Create New Product API
        /// </summary>
        /// <param name="oProduct"></param>
        /// <returns></returns>

        [HttpPost]
        public Task<IActionResult> AddNewProduct(Product oProduct)
        {
            this._repository.Add(oProduct);
            return Task.FromResult((IActionResult)Ok(oProduct));
        }
    }
}
