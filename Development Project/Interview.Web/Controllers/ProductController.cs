using Interview.Web.Models;
using Interview.Web.Repositories;
using Interview.Web.Repositories.Interfaces;
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
        private readonly IRepository<Product> _productRepo;
        public ProductController(IRepository<Product> productRepo)
        {
            _productRepo = productRepo;
        }
        // NOTE: Sample Action
        [HttpGet]
        public Task<IActionResult> GetAllProducts()
        {
            var products = _productRepo.GetAll();
            return Task.FromResult((IActionResult)Ok(products));
        }
        [HttpPost]
        public Task<IActionResult> AddProduct(Product product)
        {
            _productRepo.Insert(product);
            return Task.FromResult((IActionResult)Ok());
        }

    }
}
