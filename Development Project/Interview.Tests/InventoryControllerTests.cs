using System.Threading.Tasks;
using Interview.Web.Controllers;
using Interview.Web.Models;
using Interview.Web.Repositories;
using Interview.Web.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Interview.Tests
{
    public class InventoryControllerTests
    {
        private readonly Mock<IInventoryRepository> _mockRepo;
        private readonly InventoryController _controller;

        public InventoryControllerTests()
        {
            _mockRepo = new Mock<IInventoryRepository>();
            _controller = new InventoryController(_mockRepo.Object);
        }

        [Fact]
        public async Task GetStock_ReturnsOk_WithQuantity()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetStockAsync(1)).ReturnsAsync(50m);

            // Act
            var result = await _controller.GetStock(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<InventoryResponse>(okResult.Value);
            Assert.Equal(50m, response.CurrentStock);
        }

        [Fact]
        public async Task UpdateStock_ReturnsOk_WithTransactionId()
        {
            // Arrange
            var request = new InventoryTransactionRequest { Quantity = 10 };
            _mockRepo.Setup(r => r.AddTransactionAsync(request)).ReturnsAsync(123);

            // Act
            var result = await _controller.UpdateStock(1, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            // Dynamic object check
            Assert.NotNull(okResult.Value);
        }
    }
}
