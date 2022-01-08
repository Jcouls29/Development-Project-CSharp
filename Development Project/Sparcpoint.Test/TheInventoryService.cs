using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sparcpoint.DataServices;
using Sparcpoint.Models;
using Sparcpoint.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sparcpoint.Test
{
    [TestClass]
    public class TheInventoryService
    {
        [TestMethod]
        public async Task GetsAllCurrentInventoryTransactions()
        {
            var expectedReturnList = new List<InventoryTransactions>();

            Mock<IInventoryDataService> mockDataService = new Mock<IInventoryDataService>();
            mockDataService.Setup(p => p.GetAllInventoryTransactions()).Returns(Task.FromResult(expectedReturnList));
            var inventoryService = new InventoryService(mockDataService.Object);
            var result = await inventoryService.GetAllInventoryTransactions();
            Assert.AreEqual(expectedReturnList, result);
        }

        [TestMethod]
        public async Task GetsInventoryForProduct()
        {
            Mock<IInventoryDataService> mockDataService = new Mock<IInventoryDataService>();
            mockDataService.Setup(p => p.GetInventoryForProduct(It.IsAny<int>())).Returns(Task.FromResult(12));
            var inventoryService = new InventoryService(mockDataService.Object);
            var result = await inventoryService.GetInventoryForProduct(5);

            mockDataService.Verify(m => m.GetInventoryForProduct(5), Times.Once());
            Assert.AreEqual(12, result);
        }

        [TestClass]
        public class WhenUpdatingInventory
        {

            [TestMethod]
            public async Task AddsInventoryTransaction()
            {
                var calledWith = new InventoryTransactions();

                Mock<IInventoryDataService> mockDataService = new Mock<IInventoryDataService>();
                mockDataService.Setup(p => p.AddNewInventoryTransaction(It.IsAny<InventoryTransactions>())).Callback<InventoryTransactions>(i => { calledWith = i; }).Returns(Task.FromResult(1));
                var inventoryService = new InventoryService(mockDataService.Object);
                await inventoryService.UpdateProductInventory(6, 90);

                Assert.AreEqual(6, calledWith.ProductInstanceId);
                Assert.AreEqual(90, calledWith.Quantity);
            }
        }

        [TestClass]
        public class WhenRollingBackTRansactions
        {
            [TestMethod]
            public async Task RollsBackTheTransaction()
            {
                Mock<IInventoryDataService> mockDataService = new Mock<IInventoryDataService>();
                mockDataService.Setup(p => p.RollbackInventoryUpdate(It.IsAny<int>())).Returns(Task.CompletedTask);
                var inventoryService = new InventoryService(mockDataService.Object);
                await inventoryService.RollbackInventoryUpdate(5);

                mockDataService.Verify(m => m.RollbackInventoryUpdate(5), Times.Once());
            }
        }
    }
}
