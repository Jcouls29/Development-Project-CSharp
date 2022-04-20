using Interview.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;

namespace Interview.Web.Controllers
{
  [Route("api/v1/products")]
  public class ProductController : Controller
  {
    private ISqlExecutor _executor;

    public ProductController(ISqlExecutor executor)
    {
      _executor = executor;
    } 
    // NOTE: Sample Action
    [HttpGet]
    public async Task<IActionResult> GetAllProducts()
    {
      /// ToDo: need to add security handler
      var prods = await _executor.ExecuteAsync((connection, a) => connection.GetAllAsync<Product>(a));

      return Ok(prods);
    }
    [HttpGet("{Id}")]
    public async Task<IActionResult> GetProduct(int Id)
    {
      /// ToDo: need to add security handler
      var prod = await _executor.ExecuteAsync((connection, a) => connection.GetAsync<Product>(Id,a));

      return Ok(prod);
    }

    [HttpPut]
    public async Task<IActionResult> PutProduct([FromBody] Product product)
    {
      /// ToDo: need to add security handler
      /// 
      product.CreatedTimestamp = DateTime.Now;
      var prodId = await _executor.ExecuteAsync(
        (connection, a) => connection.InsertAsync(product,a));

      return Ok(prodId);
    }

  } 
}
