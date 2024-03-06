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
          public async Task<IActionResult> GetProducts([FromQuery] ProductSearchModel searchValues)
          {
               try
               {
                    //Check if SKU or Name are in query params
                    bool isSearchModelEmpty = string.IsNullOrWhiteSpace(searchValues.SKU) &&
                                              string.IsNullOrWhiteSpace(searchValues.Name);
                    //no query params get all
                    if (isSearchModelEmpty)
                    {
                         var products = await _productService.GetAllProductsAsync();
                         return Ok(products);
                    } 
                    else
                    {
                         //EVAL: This currently does not handle whitespace well
                         //there is also no security check for SQL injections
                         //I am turning it into a string through the search params that is not foolproof.
                         //to search with query params enter 
                         //?SKU=SKU1
                         //?Name=Example
                         //Will default to SKU if both are present
                         var product = await _productService.FindProduct(searchValues);
                         if (product == null)
                         {
                              return NotFound("No product found...");
                         }

                         return Ok(product);
                    }
               }
               catch (Exception ex)
               {
                    return StatusCode(500, "Error: Ran into an issue while retrieving products");
               }
          }

          //EVAL: Post endpoint to create product will require base product model
          // will allow for optional category and color values to be added in
          
          [HttpPost]
          public async Task<IActionResult> CreateProduct([FromBody] CreateProductModel productModel){
               
               //if body is not valid shape send back a bad request
               if (!ModelState.IsValid)
               {
                    return BadRequest(ModelState);
               }
               try
               {

                    var newProduct = await _productService.CreateProduct(productModel);
                    return new ObjectResult(newProduct) { StatusCode = 201 };
               }
               catch (Exception ex)
               {
                    return StatusCode(500, "Error: Ran into an issue while creating a new Product");
               }
          }
     }
}

//FINISHED
//was not able to get all I wanted to get done due to trying to juggle learning C#/ASP.net and Dapper
//I feel pretty good with what I have but would have liked a bit more time to setup so I could figure out
//more of my dependencies earlier on(Unit test setup, Dapper, Getting the DB to work locally)
//This was great though and lots of fun
//Thanks
