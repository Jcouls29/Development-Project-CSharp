using Interview.Web.Controllers;
using Interview.Web.Model;
using Interview.Web.Repository;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Interview.Web.Tests1
{
    public class ProductControllerTest
    {

        [Fact]
        public async Task GetProductInventoryCount_InstanceId_Valid()
        {
            // Arrange
            var mockInventory = new Mock<IInventoryRepository>();
            var mockProduct = new Mock<IProductRepository>();
            mockInventory.Setup(m => m.GetProductInventoryCount(It.IsAny<InventoryTransactions>())).ReturnsAsync(5);
            var controller = new ProductController(mockInventory.Object, mockProduct.Object);

            //Act
            var result = await controller.GetProductInventoryCount(1);


            //Assert
            var count = Assert.IsType<int>(result);
            Assert.Equal(5, count);

        }

        [Fact]
        public async Task SearchProduct__Valid()
        {
            // Arrange
            var mockInventory = new Mock<IInventoryRepository>();
            var mockProduct = new Mock<IProductRepository>();
            mockProduct.Setup(m => m.SearchProduct(It.IsAny<Product>())).ReturnsAsync(
                new List<Product> { new Product { CategoryName = "Medicine", InstanceId = 1, ProductImageUris = "url", ValidSkus = "12345", CreatedTimestamp = DateTime.Now, Description = "Cough & Fever" } ,
                 new Product { CategoryName = "Food", InstanceId = 1, ProductImageUris = "url", ValidSkus = "34536", CreatedTimestamp = DateTime.Now, Description = "Protein Bar" }});

            var controller = new ProductController(mockInventory.Object, mockProduct.Object);

            //Act
            var result = await controller.SearchProduct("Medicine");


            //Assert
            var returnValue = Assert.IsType<List<Product>>(result);
            var idea = returnValue.FirstOrDefault();
            Assert.Equal("Cough & Fever", idea.Description);

        }
    }
}
