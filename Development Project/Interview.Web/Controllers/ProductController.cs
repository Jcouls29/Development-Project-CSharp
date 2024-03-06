using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sparcpoint.SqlServer.Abstractions;
using Dapper;
using Interview.Web.Models;
using Interview.Web.Services;

//EVAL: For this test I have decided to work on
// * an endpoint to create and add Products to the database
// * searching for products by category, SKU, or color
// From the list of items I believe these to have the most return on investment
// Due to users being able to store items in our Inventory system and then allowing us to retrieve those items.
// For a POS system or inventory system these are the key elements and could easily be built upon with the new other 
// requirements in the future.

namespace Interview.Web.Controllers
{
    [Route("api/v1/products")]
    public class ProductController : Controller
    {
          private readonly ProductService _productService;

          public ProductController(ProductService productService)
          {
               _productService = productService;
          }

          [HttpGet]
          public async Task<IActionResult> GetAllProducts()
          {
               try
               {
                    var products = await _productService.GetAllProductsAsync();
                    return Ok(products);
               }
               catch (Exception ex)
               {
                    return StatusCode(500, "Error: Ran into an issue while retrieving products");
               }
          }

          //EVAL: Post endpoint to create product will require base product model
          // will allow for optional category and color values to be added in
          /*
          [HttpPost]
          public Task<IActionResult> CreateProduct(){}
          */
          /**/
          /*
          [HttpGet]
          public Task<IActionResult> SearchProducts(){}
           */
     }
}
