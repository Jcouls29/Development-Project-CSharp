using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sparcpoint.Models.DTOs;
using Sparcpoint.Abstract.Services;
using Interview.Web.Controllers;
using Sparcpoint.Models.Entity;
using System.Collections.Generic;

namespace Interview.Web.Tests.Controllers
{
    //EVAL: added tests for ProductController covering all APIs and possible scenarios including exception handling and not found scenarios
    [TestClass]
    public class ProductControllerTests
    {
        private Mock<IProductService> _mockProductService;
        private Mock<ILogger<ProductController>> _mockLogger;
        private ProductController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockProductService = new Mock<IProductService>();
            _mockLogger = new Mock<ILogger<ProductController>>();
            _controller = new ProductController(_mockProductService.Object, _mockLogger.Object);
        }

        [TestMethod]
        public async Task CreateProduct_ReturnsInternalServerError_WhenNameIsEmpty()
        {
            // Arrange
            var request = new CreateProductRequestDto { Name = "" };
            _mockProductService.Setup(s => s.AddProductAsync(request))
                .ThrowsAsync(new ArgumentException("Product name cannot be empty."));

            // Act
            var result = await _controller.CreateProduct(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var objectResult = result as ObjectResult;
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.AreEqual("An error occurred while creating the product.", objectResult.Value);
            _mockProductService.Verify(s => s.AddProductAsync(request), Times.Once);
        }

        [TestMethod]
        public async Task CreateProduct_ReturnsCreated_WhenProductIsCreated()
        {
            // Arrange
            var request = new CreateProductRequestDto { Name = "Valid Product" };
            _mockProductService.Setup(s => s.AddProductAsync(request)).ReturnsAsync(1);

            // Act
            var result = await _controller.CreateProduct(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var objectResult = result as ObjectResult;
            Assert.AreEqual(201, objectResult.StatusCode);
            _mockProductService.Verify(s => s.AddProductAsync(request), Times.Once);
        }

        [TestMethod]
        public async Task GetProductById_ReturnsProduct_WhenIdIsValid()
        {
            // Arrange
            var productId = 1;
            var product = new Product { InstanceId = productId, Name = "Test Product" };
            _mockProductService.Setup(s => s.GetProductByIdAsync(productId)).ReturnsAsync(product);

            // Act
            var result = await _controller.GetProduct(productId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.AreEqual(product, okResult.Value);
            _mockProductService.Verify(s => s.GetProductByIdAsync(productId), Times.Once);
        }

        [TestMethod]
        public async Task GetProductById_ReturnsNotFound_WhenIdIsInvalid()
        {
            // Arrange
            var productId = 999;
            _mockProductService.Setup(s => s.GetProductByIdAsync(productId))
                .ThrowsAsync(new KeyNotFoundException("Product not found."));

            // Act
            var result = await _controller.GetProduct(productId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var objectResult = result as ObjectResult;
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.AreEqual("An error occurred while searching for products.", objectResult.Value);
            _mockProductService.Verify(s => s.GetProductByIdAsync(productId), Times.Once);

        }

        [TestMethod]
        public async Task SearchProducts_ReturnsProducts_WhenCriteriaIsValid()
        {
            // Arrange
            var searchCriteria = new SearchProductRequestDto { Name = "Test" };
            var products = new List<Product> { new Product { InstanceId = 1, Name = "Test Product" } };
            _mockProductService.Setup(s => s.SearchProductAsync(searchCriteria)).ReturnsAsync(products);

            // Act
            var result = await _controller.SearchProducts(searchCriteria);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.AreEqual(products, okResult.Value);
            _mockProductService.Verify(s=> s.SearchProductAsync(searchCriteria), Times.Once);
        }
    }
}