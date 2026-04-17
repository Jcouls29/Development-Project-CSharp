using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Interview.Web.Controllers;
using Interview.Web.Models;
using Interview.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Interview.Web.Tests
{
    public class InventoryControllerTests
    {
        [Fact]
        public async Task AddInventory_ReturnsOkWithTransaction()
        {
            var mockSvc = new Mock<IInventoryService>();
            var productId = Guid.NewGuid();
            var tx = new InventoryTransaction { Id = Guid.NewGuid(), ProductId = productId, Delta = 4, Note = "restock" };
            mockSvc.Setup(s => s.AddInventoryAsync(productId, 4, "restock")).ReturnsAsync(tx);

            var controller = new InventoryController(mockSvc.Object);
            var req = new InventoryController.InventoryRequest { Delta = 4, Note = "restock" };

            var result = await controller.AddInventory(productId, req);

            var ok = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<InventoryTransaction>(ok.Value);
            Assert.Equal(4, returned.Delta);
        }

        [Fact]
        public async Task GetQuantity_ReturnsOkWithQuantity()
        {
            var mockSvc = new Mock<IInventoryService>();
            var productId = Guid.NewGuid();
            mockSvc.Setup(s => s.GetQuantityAsync(productId)).ReturnsAsync(5);

            var controller = new InventoryController(mockSvc.Object);

            var result = await controller.GetQuantity(productId);

            var ok = Assert.IsType<OkObjectResult>(result);
            var obj = ok.Value as dynamic;
            Assert.Equal(productId, (Guid)obj.productId);
            Assert.Equal(5, (int)obj.quantity);
        }

        [Fact]
        public async Task GetQuantityByCriteria_ReturnsSum()
        {
            var mockSvc = new Mock<IInventoryService>();
            mockSvc.Setup(s => s.GetQuantityByMetadataAsync(It.IsAny<Dictionary<string, string>>())).ReturnsAsync(12);

            var controller = new InventoryController(mockSvc.Object);

            var result = await controller.GetQuantityByCriteria(new Dictionary<string, string> { { "sku", "X" } });

            var ok = Assert.IsType<OkObjectResult>(result);
            var obj = ok.Value as dynamic;
            Assert.Equal(12, (int)obj.quantity);
        }

        [Fact]
        public async Task UndoInventory_ReturnsNoContentWhenOk()
        {
            var mockSvc = new Mock<IInventoryService>();
            var productId = Guid.NewGuid();
            mockSvc.Setup(s => s.UndoLastInventoryAsync(productId)).ReturnsAsync(true);

            var controller = new InventoryController(mockSvc.Object);

            var result = await controller.UndoInventory(productId);

            Assert.IsType<NoContentResult>(result);
        }
    }
}
