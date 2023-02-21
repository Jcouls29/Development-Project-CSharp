﻿using Interview.Web.Services;
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
            var products = new List<Product>()
            {
                new Product(){
                    ProductId = Guid.NewGuid(),
                    Name = "Existing Product 1",
                    Description = "I'm a description",
                    Categories = new Dictionary<string, int>() { { "Blue", 1 }, { "Sale", 1 } },
                    MetaData = new Dictionary<string, object>() { { "Sku", 19932 }, { "Color", "Blue" } },
                    IsDeleted = false
                },
                new Product(){
                    ProductId = Guid.NewGuid(),
                    Name = "Existing Product 2",
                    Description = "I'm a description for #2",
                    Categories = new Dictionary<string, int>() { { "Red", 2 }, { "Sale", 0 } },
                    MetaData = new Dictionary<string, object>() { { "Sku", 4566 }, { "Color", "Red" } },
                    IsDeleted = false
                },
                new Product(){
                    ProductId = Guid.NewGuid(),
                    Name = "Existing Product 3",
                    Description = "I'm a description but longer",
                    Categories = new Dictionary<string, int>() { { "Green", 3 }, { "Sale", 0 } },
                    MetaData = new Dictionary<string, object>() { { "Sku", 4654 }, { "Color", "Green" } },
                    IsDeleted = false
                },
                new Product(){
                    ProductId = Guid.NewGuid(),
                    Name = "Existing Product 4",
                    Description = "I'm a description but I was deleted",
                    Categories = new Dictionary<string, int>() { { "Purple", 5 }, { "Sale", 2 } },
                    MetaData = new Dictionary<string, object>() { { "Sku", 463454 }, { "Color", "Purple" } },
                    IsDeleted = true
                }
            };

            return await Task.FromResult((IActionResult)Ok(products.Where(x => !x.IsDeleted)));
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
