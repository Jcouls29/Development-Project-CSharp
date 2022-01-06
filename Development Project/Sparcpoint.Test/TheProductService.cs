using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sparcpoint.DataServices;
using Sparcpoint.Models;
using Sparcpoint.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sparcpoint.Test
{
    [TestClass]
    public class TheProductService
    {
        [TestMethod]
        public async Task GetsAllCurrentProducts()
        {
            var expectedReturnList = new List<Product>();

            Mock<IProductDataService> mockDataService = new Mock<IProductDataService>();
            mockDataService.Setup(p => p.GetProducts()).Returns(Task.FromResult(expectedReturnList));
            var productService = new ProductService(mockDataService.Object);
            var result = await productService.GetProducts();
            Assert.AreEqual(expectedReturnList, result);
        }

        [TestMethod]
        public async Task SavesAProductToTheDataService()
        {
            Product calledWith = new Product();
            Mock<IProductDataService> mockDataService = new Mock<IProductDataService>();
            mockDataService.Setup(p => p.CreateProductAsync(It.IsAny<Product>())).Callback<Product>(p => calledWith = p);
            var productService = new ProductService(mockDataService.Object);

            var productRequest = new CreateProductRequest()
            {
                Name = "super awesome name",
                Description = "The best description ever",
                ProductImageUris = new List<string>() { "www.google.com/cutekittens", "www.google.com/cutepuppies" },
                ValidSkus = new List<string>() { "1234", "5678" }
            };

            await productService.CreateProductAsync(productRequest);

            Assert.AreEqual(productRequest.Name, calledWith.Name);
            Assert.AreEqual(productRequest.Description, calledWith.Description);
            Assert.AreEqual(String.Join(",", productRequest.ProductImageUris.ToArray()), calledWith.ProductImageUris);
            Assert.AreEqual(String.Join(",", productRequest.ValidSkus.ToArray()), calledWith.ValidSkus);

        }
    }
}
