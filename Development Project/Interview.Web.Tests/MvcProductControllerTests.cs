using Microsoft.VisualStudio.TestTools.UnitTesting;
using Interview.Web.Controllers;
using Moq;
using Dal.Interfaces;
using Dal.Models;
using System.Collections.Generic;
using System.Linq;
using DomainServices.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Interview.Web.Tests
{
    [TestClass]
    public class MvcProductControllerTests
    {
        private Mock<IProductService> _productService;
        public MvcProductControllerTests()
        {
            _productService = new Mock<IProductService>();
        }


        [TestMethod]
        public void MvcProductController_Should_Return_One_Product()
        {
            var mockProducts = new List<Products>() {
            new Products(){
                InstanceId = 1,
                Name = "Macbook Pro",
                CategoryName = "Laptops",
                Description = "Macbook",
                ProductImageUris = "www.apple.com",
                ValidSkus = "Color, Model, Serial Number",
                CreateTimestamp = System.DateTime.Now
                }
            };

            _productService.Setup(pr => pr.GetProducts()).Returns(mockProducts);
            var productController = new ProductController(_productService.Object);
            var viewResult = productController.Index() as ViewResult;
            var products = (List<Products>)viewResult.ViewData.Model;

            Assert.IsNotNull(products);
            Assert.AreEqual(1, products.Count);
            Assert.IsTrue(products[0].CategoryName == "Laptops");
        }
    }
}
