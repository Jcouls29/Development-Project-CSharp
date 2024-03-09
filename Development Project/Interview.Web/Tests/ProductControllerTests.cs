using NUnit.Framework;
using Moq;
using System.Threading.Tasks;
using Interview.Web.Controllers;
using Interview.Web.Services;
using Interview.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Interview.Web.Tests
{
    [TestFixture]
    public class ProductControllerTests
    {
        private Mock<ClientsProductService> _clientsProductServiceMock;
        private ProductController _productController;

        [SetUp]
        public void Setup()
        {
            _clientsProductServiceMock = new Mock<ClientsProductService>();
            _productController = new ProductController(_clientsProductServiceMock.Object);
        }

        [Test]
        public async Task GetProducts_NoSearchCriteria_Returns_AllProducts()
        {
            // Arrange
            var searchValues = new ProductSearchModel();
            var allProducts = new List<Product>();
            _clientsProductServiceMock.Setup(m => m.GetAllProductsAndCorrespondingCategory()).ReturnsAsync(allProducts);

            // Act
            var result = await _productController.GetProducts(searchValues);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.AreEqual(allProducts, (result as OkObjectResult).Value);
        }

        [Test]
        public async Task GetProducts_WithSearchCriteria_Returns_FilteredProducts()
        {
            // Arrange
            var searchValues = new ProductSearchModel { SKU = "123" };
            var filteredProducts = new List<Product> { new Product { Id = 1, Name = "Product1" } };
            _clientsProductServiceMock.Setup(m => m.SearchProductsAsync(searchValues)).ReturnsAsync(filteredProducts);

            // Act
            var result = await _productController.GetProducts(searchValues);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.AreEqual(filteredProducts, (result as OkObjectResult).Value);
        }

        [Test]
        public async Task CreateProduct_With_ValidData_Returns_CreatedAtActionResult()
        {
            // Arrange
            var productModel = new CreateProductDetailsModel { Name = "Product1" };
            var createdProduct = new Product { Id = 1, Name = "Product1" };
            _clientsProductServiceMock.Setup(m => m.CreateProductAsync(productModel)).ReturnsAsync(createdProduct);

            // Act
            var result = await _productController.CreateProduct(productModel);

            // Assert
            Assert.IsInstanceOf<CreatedAtActionResult>(result);
            Assert.AreEqual(nameof(ProductController.GetProductById), (result as CreatedAtActionResult).ActionName);
            Assert.AreEqual(createdProduct, (result as CreatedAtActionResult).Value);
        }

        [Test]
        public async Task GetProductById_With_ExistingId_Returns_OkResult()
        {
            // Arrange
            int productId = 1;
            var product = new Product { Id = productId, Name = "Product1" };
            _clientsProductServiceMock.Setup(m => m.GetProductByIdAsync(productId)).ReturnsAsync(product);

            // Act
            var result = await _productController.GetProductById(productId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.AreEqual(product, (result as OkObjectResult).Value);
        }

        [Test]
        public async Task GetProductById_With_NonExistingId_Returns_NotFoundResult()
        {
            // Arrange
            int productId = 1;
            _clientsProductServiceMock.Setup(m => m.GetProductByIdAsync(productId)).ReturnsAsync((Product)null);

            // Act
            var result = await _productController.GetProductById(productId);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public async Task AddToInventory_With_ValidData_Returns_OkResult()
        {
            // Arrange
            int productId = 1;
            int quantity = 10;

            // Act
            var result = await _productController.AddToInventory(productId, quantity);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.AreEqual($"Added {quantity} units to the inventory for product with ID {productId}", (result as OkObjectResult).Value);
        }

        [Test]
        public async Task RemoveFromInventory_With_ValidData_Returns_OkResult()
        {
            // Arrange
            int productId = 1;
            int quantity = 5;

            // Act
            var result = await _productController.RemoveFromInventory(productId, quantity);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.AreEqual($"Removed {quantity} units from the inventory for product with ID {productId}", (result as OkObjectResult).Value);
        }

        [Test]
        public async Task GetProductInventoryCount_With_ValidIdentifier_Returns_OkResult()
        {
            // Arrange
            string identifier = "ABC123";
            var inventoryCount = 50;
            _clientsProductServiceMock.Setup(m => m.GetProductInventoryCountAsync(identifier)).ReturnsAsync(inventoryCount);

            // Act
            var result = await _productController.GetProductInventoryCount(identifier);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.AreEqual(inventoryCount, (result as OkObjectResult).Value);
        }

    }
}
