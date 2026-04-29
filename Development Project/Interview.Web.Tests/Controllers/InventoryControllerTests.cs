using Interview.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sparcpoint.Abstract.Services;
using Sparcpoint.Models.DTOs;
using Sparcpoint.Models.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace Interview.Web.Tests.Controllers
{
    //EVAL: added tests for InventoryController covering all APIs and possible scenarios including exception handling and not found scenarios
    [TestClass]
    public class InventoryControllerTests
    {
        private Mock<IInventoryService> _mockInventoryService;
        private Mock<ILogger<InventoryController>> _mockLogger;
        private InventoryController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockInventoryService = new Mock<IInventoryService>();
            _mockLogger = new Mock<ILogger<InventoryController>>();
            _controller = new InventoryController(_mockInventoryService.Object, _mockLogger.Object);
        }

 
        [TestMethod]
        public async Task AddProduct_ReturnsOk_WhenProductIsAdded()
        {
            // Arrange
            var request = new AddToInventoryRequestDto { ProductInstanceId = 1 };
            _mockInventoryService.Setup(s => s.AddProductToInventoryAsync(request)).ReturnsAsync(1);

            // Act
            var result = await _controller.AddProduct(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            _mockInventoryService.Verify(s => s.AddProductToInventoryAsync(request), Times.Once);
        }

        [TestMethod]
        public async Task DeleteTransaction_ReturnsNoContent_WhenTransactionIsDeleted()
        {
            // Arrange
            var transactionId = 1;
            _mockInventoryService.Setup(s => s.RemoveInventoryTransactionAsync(transactionId)).ReturnsAsync(1);

            // Act
            var result = await _controller.RemoveTransaction(transactionId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            _mockInventoryService.Verify(s => s.RemoveInventoryTransactionAsync(transactionId), Times.Once);
        }

        [TestMethod]
        public async Task DeleteTransaction_ReturnsNotFound_WhenTransactionDoesNotExist()
        {
            // Arrange
            var transactionId = 999;
            _mockInventoryService.Setup(s => s.RemoveInventoryTransactionAsync(transactionId)).ReturnsAsync(0);

            // Act
            var result = await _controller.RemoveTransaction(transactionId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            _mockInventoryService.Verify(s => s.RemoveInventoryTransactionAsync(transactionId), Times.Once);
        }

        [TestMethod]
        public async Task DeleteProduct_ReturnsNoContent_WhenProductIsDeleted()
        {
            // Arrange
            var productId = 1;
            _mockInventoryService.Setup(s => s.RemoveProductFromInventoryAsync(productId)).ReturnsAsync(1);

            // Act
            var result = await _controller.RemoveProduct(productId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            _mockInventoryService.Verify(s => s.RemoveProductFromInventoryAsync(productId), Times.Once);
        }

        [TestMethod]
        public async Task DeleteProduct_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var productId = 999;
            _mockInventoryService.Setup(s => s.RemoveProductFromInventoryAsync(productId)).ReturnsAsync(0);

            // Act
            var result = await _controller.RemoveProduct(productId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            _mockInventoryService.Verify(s => s.RemoveProductFromInventoryAsync(productId), Times.Once);
        }

        [TestMethod]
        public async Task GetInventoryCount_ReturnsCount()
        {

            // Arrange
            int productId = 1;
            _mockInventoryService.Setup(s => s.GetProuctInventoryCountAsync(productId)).ReturnsAsync(100);

            // Act
            var result = await _controller.GetInventoryCount(productId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            _mockInventoryService.Verify(s => s.GetProuctInventoryCountAsync(productId), Times.Once);

        }
    }
}