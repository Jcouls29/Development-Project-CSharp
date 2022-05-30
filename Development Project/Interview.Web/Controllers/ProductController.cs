using Interview.Web.Helpers;
using Interview.Web.Interfaces;
using Interview.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepo;

        public ProductController(IProductRepository productRepository)
        {
            _productRepo = productRepository;
        }

        /// <summary>
        /// EVAL: Checks for availablity:  This is a ping
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("IsAvailable")]
        public IActionResult IsAvailable()
        {
            return Ok(Constants.MessageIsAvailable);
        }


        /// <summary>
        /// EVAL: Get All Products not marked as 'DELETED'
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllProducts")]
        public Task<IActionResult> GetAllProducts()
        {
            return Task.FromResult((IActionResult)Ok(_productRepo.GetAll()));
        }

        [HttpGet]
        [Route("GetInventory")]
        public Task<IActionResult> GetInventory([FromBody] Product product)
        {
            if (product == null)
            {
                return Task.FromResult((IActionResult)BadRequest("Invalid"));
            }
            else
            {
                return Task.FromResult((IActionResult)Ok(_productRepo.GetInventory(product)));
            }
        }

        [HttpGet]
        [Route("GetInventoryByOptions/{Name:alpha?}/{Description:alpha?}/{ValidSkus:alpha?}/{CategoryName:alpha?}")]

        public Task<IActionResult> GetInventoryByOptions(string Name, string Description, string ValidSkus, string CategoryName)
        {
            return Task.FromResult((IActionResult)Ok(_productRepo.GetInventory(Name, Description, ValidSkus, CategoryName)));
        }


        [HttpGet]
        [Route("GetProductCount")]
        public Task<IActionResult> GetProductCount()
        {
            return Task.FromResult((IActionResult)Ok(_productRepo.GetActiveProductCount()));
        }


        /// <summary>
        /// Add product (and the following following tables: Instances.Categories and Instances.ProductCategories
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddProduct")]
        public Task<IActionResult> AddProduct([FromBody] Product product)
        {
            if (product == null)
            {
                return Task.FromResult((IActionResult)BadRequest("Invalid")); ;
            }
            else
            {
                return Task.FromResult((IActionResult)Ok(_productRepo.Add(product)));
            }
        }



        /// <summary>
        /// EVAL:  Mark the product as "DELETED" (according to requirements do not delete from table)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("DeleteProduct/{id}")]
        public Task<IActionResult> DeleteProduct(int? id)
        {
            if (id == null)
            {
                return Task.FromResult((IActionResult)BadRequest("Invalid"));
            }

            _productRepo.Remove(id.GetValueOrDefault());
            return Task.FromResult((IActionResult)Ok());
        }
    }
}
