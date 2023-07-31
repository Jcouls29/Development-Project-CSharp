using Interview.Web.Services.interfaces;
using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Controllers
{
    [Route("api/v1/products")]
    public class ProductController : Controller
    {
        public readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [Route("api/v1/products/all")]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllProduct();

            return await Task.FromResult((IActionResult)Ok(products));
        }

        [HttpPost]
        [Route("api/v1/products/add")]
        public async Task<IActionResult> AddProduct([FromBody] ProductModel productModel)
        {
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
            //EVAL: Product has remove boolean 
            if (!await _productService.ProductExists(productId))
                return await Task.FromResult(NotFound());

            var deletedProduct = await _productService.DeleteProduct(productId);

            return await Task.FromResult(Ok(deletedProduct));
        }

        //EVAL: Needs to to implemented later
        [HttpPost]
        [Route("api/v1/products/search")]
        public async Task<IActionResult> FindProduct(List<string> searchTerms)
        {
            return await Task.FromResult((IActionResult)Ok(new object[] { }));
        }
    }
}
