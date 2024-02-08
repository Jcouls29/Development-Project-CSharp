using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Data;
using System.Data.SqlClient;
using Sparcpoint.SqlServer.Abstractions;
using Interview.Web.Controllers;
using Sparcpoint.SqlServer.Abstractions.Data;
using Microsoft.AspNetCore.Mvc;

namespace Interview.Web.Test
{
    [TestClass]
    public class TransactionsTests
    {
        private ISqlExecutor sqlExecutor;
        private TransactionsController controller;

        [TestInitialize]
        public void TestInitialize()
        {
            // Arrange (Common)
            sqlExecutor = new SqlServerExecutor(@"Server=localhost\SQLEXPRESS;Database=master;Trusted_Connection=True;");
            controller = new TransactionsController(sqlExecutor);
        }

        /// <summary>
        /// Requirement: Adding product definitions into Inventory.
        /// </summary>
        [TestMethod]
        public void CreateInventoryTransaction_Returns_Ok()
        {
            // Arrange
            var inventoryTransaction = new InventoryTransaction
            {
                ProductInstanceId = 1,
                Quantity = 11,
                StartedTimestamp = DateTime.UtcNow,
                CompletedTimestamp = DateTime.UtcNow,
                TypeCategory = "A"
            };

            // Act
            var result = controller.CreateInventoryTransaction(inventoryTransaction);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }

        [TestMethod]
        public void GetInventoryCount_Returns_Ok()
        {
            // Act
            var result = controller.GetInventoryCount(productInstanceId: "1");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        /// <summary>
        /// Requirement: Removing products from Inventory - one transaction
        /// </summary>
        [TestMethod]
        public void RemoveInventory_Returns_Ok()
        {
            // Act
            var result = controller.RemoveInventory("1");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }

    }
}
