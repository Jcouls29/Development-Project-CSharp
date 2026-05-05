using Interview.Web.Contexts;
using Interview.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Interview.Web.Contexts;
using Microsoft.Extensions.Logging;

/*
 * EVAL: Was not aware that Visual Studio would be a requirement. I am running on Endeavour
 *       and, as such, do not have access to Visual Studio. Sparcpoint.Inventory.Database
 *       does not load in Rider. I'm doing my best to extrapolate what I can from the
 *       *.sqlproj file and associated *.sql files.
 *
 *       ERROR: "$(MSBuildExtensionsPath)/Microsoft/VisualStudio/v$(VisualStudioVersion)/SSDT/Microsoft.Data.Tools.Schema.SqlTasks.targets"
 *              does not exist and appears to be part of a Visual Studio component. This file may require MSBuild.exe in order to be imported
 *              successfully, and so may fail to build in the dotnet CLI.
 *              /home/jcroft/Git/Development-Project-CSharp/Development Project/Sparcpoint.Inventory.Database/Sparcpoint.Inventory.Database.sqlproj at (84:3)
 */

namespace Interview.Web.Controllers
{
    [ApiController]
    [Route("api/v1/products")]
    public class ProductController : ControllerBase
    {
        private ILogger<ProductController> _logger;
        
        public ProductController(ILogger<ProductController> logger)
        {
            _logger = logger;    
        }
        
        // NOTE: Sample Action
        [HttpGet]
        public ActionResult<List<ProductModel>> GetAllProducts()
        {
            try
            {
                var products = ProductContext.GetAllProducts();
                return Ok(products);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return Problem();
            }
        }

        [HttpPost]
        public ActionResult<ProductModel> CreateProduct([FromBody] CreateProductDto product)
        {
            try
            {
                var newProd = ProductContext.CreateProduct(product);
                return Ok(newProd);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return Problem();
            }
        }
    }
}
