using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Abstract;
using Sparcpoint.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [Route("api/v1/[controller]/[action]")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        public ProductController()
        {

        }
        [HttpGet]
        public IActionResult GetAllProducts()
        {
            return Ok(_productService.GetAllProducts());
        }


        [HttpPost]
        public IActionResult AddProduct(ProductDto product)
        {
            return Ok(_productService.AddProduct(product));
        }


        [HttpPost]
        public IActionResult AddProduct(List<int> id)
        {
            _productService.RemoveProduct(id);
            return Ok();
        }



        [HttpPost]
        public IActionResult AddToInventory(int id, InventoryTransactionDto inventory)
        {
            return Ok(_productService.AddToInventory(id, inventory));
        }


        [HttpGet]
        public IActionResult Count(string input, CountType countType)
        {
            return Ok(_productService.Count(input, countType));
        }

        [HttpDelete("{id}")]
        public IActionResult RemoveTransaction(int id)
        {
            return Ok(_productService.RemoveTransaction(id));
        }

        [HttpGet]
        public IActionResult SearchProducts(string input)
        {
            return Ok(_productService.SearchProducts(input));
        }
    }
}
