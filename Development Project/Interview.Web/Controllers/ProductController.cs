using Microsoft.AspNetCore.Mvc;
using Sparcpoint.API.Models;
using System;
using System.Threading.Tasks;
using Sparcpoint.API.API;

namespace Interview.Web.Controllers
{
    [Route("api/v1/products")]
    public class ProductController(Products products) : Controller
    {

        // NOTE: Sample Action
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var allProducts = await products.GetProducts();

            return Ok(allProducts);
        }

        [HttpPost]
        public async Task<IActionResult> AddNewProduct(ProductModel productModel, ProductAttributesModel productAttributesModel)
        {
            var add = await products.AddProduct(productModel, productAttributesModel);

            return Json(add);
        }

        [HttpPost]
        public async Task<IActionResult> SearchProducts(SearchModel searchModel)
        {
            var search = await products.SearchProducts(searchModel);

            //get needed product info to pass into ui

            return Json(search);
        }
    }
}
