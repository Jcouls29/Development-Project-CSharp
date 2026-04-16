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
    public class ProductControllerTests
    {
        [Fact]
        public async Task CreateProduct_Returns_CreatedAtAction()
        {
            // Arrange
            var mockService = new Mock<IProductService>();
            var product = new Product { Id = Guid.NewGuid(), Name = "Controller Test" };
            mockService.Setup(s => s.CreateProductAsync(It.IsAny<Product>())).ReturnsAsync(product);

            var controller = new ProductController(mockService.Object);

            // Act
            var result = await controller.CreateProduct(product);

            // Assert
            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(ProductController.GetById), created.ActionName);
            var returned = Assert.IsType<Product>(created.Value);
            Assert.Equal(product.Id, returned.Id);
        }

        [Fact]
        public async Task GetAllProducts_ReturnsOkWithList()
        {
            var mockService = new Mock<IProductService>();
            var list = new List<Product> { new Product { Name = "P1" } };
            mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(list);

            var controller = new ProductController(mockService.Object);

            var result = await controller.GetAllProducts();

            var ok = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsAssignableFrom<IEnumerable<Product>>(ok.Value);
            Assert.Single(returned);
        }
    }
}
