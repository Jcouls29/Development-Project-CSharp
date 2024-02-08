using Interview.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sparcpoint.SqlServer.Abstractions;
using Sparcpoint.SqlServer.Abstractions.Data;
using System;
using System.Collections.Generic;
using System.Data;

namespace Interview.Web.Test
{
    [TestClass]
    public class InstancesTests
    {

        private ISqlExecutor sqlExecutor;
        private InstancesController controller;

        [TestInitialize]
        public void TestInitialize()
        {
            // Arrange (Common)
            sqlExecutor = new SqlServerExecutor(@"Server=localhost\SQLEXPRESS;Database=master;Trusted_Connection=True;");
            controller = new InstancesController(sqlExecutor);
        }

        [TestMethod]
        public void AddProduct_Returns_Ok()
        {
            // Arrange
            var product = new Product
            {
                Name = "Pass me in",
                Description = "Test product",
                ProductImageUris = "TEST.com,TEST2.com",
                ValidSkus = "ABC123",
                CreatedTimestamp = DateTime.UtcNow
            };

            // Act
            var result = controller.CreateProduct(product);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }

        [TestMethod]
        public void AddTestProduct_Returns_Ok()
        {
            // Act
            var result = controller.CreateTestProduct();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }

        [TestMethod]
        public void AddTestProduct_Returns_Ok_Mock()
        {
            // Arrange
            var mockExecutor = new Mock<ISqlExecutor>();
            mockExecutor.Setup(e => e.Execute(It.IsAny<Func<IDbConnection, IDbTransaction, int>>()))
                .Returns(1); // Assuming one row affected

            var controllerWithMockExecutor = new InstancesController(mockExecutor.Object);

            // Act
            var result = controllerWithMockExecutor.CreateTestProduct();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }

        [TestMethod]
        public void GetAllProducts_Returns_Products()
        {
            // Arrange
            // Ensure there is at least one product.
            var product = new Product
            {
                Name = "Pass me in",
                Description = "Test product",
                ProductImageUris = "TEST.com,TEST2.com",
                ValidSkus = "ABC123",
                CreatedTimestamp = DateTime.UtcNow
            };
            controller.CreateProduct(product);

            // Act
            var result = controller.GetAllProducts();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));

            // Act
            var okResult = result as OkObjectResult;

            // Assert
            Assert.IsNotNull(okResult.Value);

            // Act
            var productList = okResult.Value as List<Product>;
            Assert.IsNotNull(productList);
            Assert.IsTrue(productList.Count > 0);
        }

        [TestMethod]
        public void GetProductInstanceIdFromName_OK()
        {
            // Act 
            var result = controller.GetProductInstanceIdFromName("Test Name");
            var okResult = result as OkObjectResult;
            var instanceId = okResult.Value;
            Assert.IsNotNull(instanceId);
        }

        [TestMethod]
        public void Demo()
        {
            var productName = "Cozy Cottage";
            var product = new Product
            {
                Name = productName,
                Description = "",
                ProductImageUris = "",
                ValidSkus = "COT",
                CreatedTimestamp = DateTime.UtcNow
            };

            // A working system would probably combine various actions
            // such as adding products, categories, and so on.

            var result = controller.CreateProduct(product); // Would be nice if this returned the instance id. Another API layer for such queries.

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkResult));

            // I tried to return the identity column. 
            //Assert.IsInstanceOfType(result, typeof(ObjectResult));
            //var okResult = result as OkObjectResult;
            //var productInstanceIdObject = okResult.Value;
            //var productInstanceId = (int)(((OkObjectResult)productInstanceIdObject).Value);
            //Assert.IsNotNull(productInstanceId);
            //Assert.IsInstanceOfType(productInstanceId, typeof(int));

            // Get the instance id from a name.
            // Maybe we get it from the UI.
            var getProductInstanceResult = controller.GetProductInstanceIdFromName(productName);
            Assert.IsInstanceOfType(getProductInstanceResult, typeof(OkObjectResult));

            int productInstanceIdAnotherWay = (int)(((OkObjectResult)getProductInstanceResult).Value);
            Assert.IsNotNull(productInstanceIdAnotherWay);
            Assert.IsInstanceOfType(productInstanceIdAnotherWay, typeof(int));

            // Let's create a category, 
            var category = new Category()
            {
                Name = "Sample Category",
                Description = "This is a sample category description.",
                CreatedTimestamp = DateTime.UtcNow
            };
            var resultCategory = controller.CreateCategory(category);
            Assert.IsNotNull(resultCategory);
            Assert.IsInstanceOfType(result, typeof(OkResult));

            var categoryInstanceId = 0;

            // Let's then associate it with the above product via the productInstanceId.

            var productCategory = new ProductCategory()
            {
                InstanceId = productInstanceIdAnotherWay,
                CategoryInstanceId = categoryInstanceId
            };
            var ceateCategoryCategoryResult = controller.CreateProductCategory(productCategory);
        }
    }
}
