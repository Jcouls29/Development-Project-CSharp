using Interview.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Entities;
using Sparcpoint.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Interview.Web.Controllers
{

    public class ProductController : Controller
    {
        public readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // NOTE: Sample Action
        [HttpGet]
        [Route("api/v1/products/all")]
        public async Task<IActionResult> GetAllProducts()
        {
            //EVAL: Assuming we only want products not deleted (IsDeleted = False)
            var products = await _productService.AllProduct();

            return await Task.FromResult((IActionResult)Ok(products));
        }

        [HttpPost]
        [Route("api/v1/products/add")]
        public async Task<IActionResult> AddProduct([FromBody] ProductModel productModel)
        {
            //EVAL: To keep controllers thin, error handling should be handled in a middleware. Not using a try/catch here and letting the built in handlers return
            if (!await _productService.ProductExists(productModel.ProductId))
                return await Task.FromResult(NoContent());
            
            var addedProduct = await _productService.AddProduct(productModel);
            
            return await Task.FromResult(Ok(addedProduct));
            
        }

        [HttpPost]
        [Route("api/v1/products/update")]
        public async Task<IActionResult> UpdateProduct([FromBody] ProductModel productModel)
        {
            if (!await _productService.ProductExists(productModel.ProductId))
                return await Task.FromResult(NotFound());

            var updatedProduct = await _productService.UpdateProduct(productModel);

            return await Task.FromResult(Ok(updatedProduct));
        }

        [HttpDelete]
        [Route("api/v1/products/delete")]
        public async Task<IActionResult> DeleteProduct(Guid productId)
        {
            //EVAL: Product should have a IsDeleted bool flag
            if (!await _productService.ProductExists(productId))
                return await Task.FromResult(NotFound());

            var deletedProduct = await _productService.DeleteProduct(productId);

            return await Task.FromResult(Ok(deletedProduct));
        }

        [HttpPost]
        [Route("api/v1/products/search")]
        public async Task<IActionResult> FindProduct(List<string> searchTerms)
        {
            //EVAL: This will be a stored proc, which I haven't written. 
            return await Task.FromResult((IActionResult)Ok(new object[] { }));
        }
    }
}
