using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sparcpoint.SqlServer.Abstractions;
using Dapper;
using Interview.Web.Models;

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
        private readonly ISqlExecutor _sqlExecutor;

          public ProductController(ISqlExecutor sqlExecutor)
          {
               _sqlExecutor = sqlExecutor;
          }

          [HttpGet]
          public async Task<IActionResult> GetAllProducts()
          {
               //used this to test DB connection
               //TODO: Move to service file for GetAll
               //If you have time add in necessary params or access haha
               try
               {
                    var products = await _sqlExecutor.ExecuteAsync(async (conn, trnx) =>
                    {
                         var result = await conn.QueryAsync<Product>(sql: @"SELECT * FROM inventory.Instances.Products;", transaction: trnx);
                         return result.ToList();
                    });

                    if (products == null || !products.Any())
                    {
                         return Ok(new List<Product>());
                    }

                    return Ok(products);
               }
               catch (Exception ex)
               {

                    return StatusCode(500, "An error occurred while processing your request.");
               }
          }

          //EVAL: Post endpoint to create product will require base product model
          // will allow for optional category and color values to be added in
          /*
          [HttpPost]
          public Task<IActionResult> CreateProduct(){}
          */
          /*
          [HttpGet]
          public Task<IActionResult> SearchProducts(){}
           */
     }
}
