using Interview.Web.ApiControllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Dal.Interfaces;
using Dal.Models;
using System.Collections.Generic;
using System.Linq;

namespace Interview.Web.Tests
{
    [TestClass]
    public class ApiProductControllerTests
    {
        private ProductController _productController;
        private Mock<IProductRepository> _productRepository;

        public ApiProductControllerTests()
        {
            _productRepository = new Mock<IProductRepository>();           
        }

        [TestMethod]
        public void ApiProductController_Should_Return_Product()
        {
            _productRepository.Setup(pr => pr.GetProducts()).Returns(new List<Products>() {
            new Products(){
                InstanceId = 1,
                Name = "Macbook Pro",
                CategoryName = "Laptops",
                Description = "Macbook",
                ProductImageUris = "www.apple.com",
                ValidSkus = "Color, Model, Serial Number",
                CreateTimestamp = System.DateTime.Now
                }
            });

            _productController = new ProductController(_productRepository.Object);

            var products = _productController.Get();

            Assert.IsNotNull(products);
            Assert.AreEqual(products.ToList().Count, 1);
            Assert.AreEqual(products.ToList()[0].CategoryName, "Laptops");
        }
    }
}
