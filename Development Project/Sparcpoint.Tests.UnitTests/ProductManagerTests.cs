using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sparcpoint.Exceptions;
using Sparcpoint.Products.Data;
using Sparcpoint.Products.Domain;
using Sparcpoint.Tests.UnitTests.Mocks;
using System;
using System.Threading.Tasks;

namespace Sparcpoint.Tests.UnitTests
{
    [TestClass]
    public class ProductManagerTests
    {
        [TestMethod]
        [ExpectedException(typeof(ItemMissingException))]
        public async Task GetProductById_WhenProductNotExists_ShouldThrowException()
        {
            // Eval: The advantage here is that we can now use straightforward inheritance to create our tests.
            // Tests become more readable and easier to understand.
            ProductManagerMock sut = new ProductManagerMock("");
            await sut.GetProductById(0);
        }

        [TestMethod]
        [ExpectedException(typeof(ParameterRequiredException))]
        public async Task AddNewProduct_WhenMissingManufacturer_ShouldThrowException()
        {
            ProductManagerMock sut = new ProductManagerMock("");
            await sut.AddNewProduct(new Product { ModelName = "Foo", Description = "Bar"});
        }

        [TestMethod]
        [ExpectedException(typeof(ParameterRequiredException))]
        public async Task AddNewProduct_WhenMissingModel_ShouldThrowException()
        {
            ProductManagerMock sut = new ProductManagerMock("");
            await sut.AddNewProduct(new Product { Manufacturer = "Foo", Description = "Bar" });
        }

        [TestMethod]
        [ExpectedException(typeof(ParameterRequiredException))]
        public async Task AddNewProduct_WhenMissingDescription_ShouldThrowException()
        {
            ProductManagerMock sut = new ProductManagerMock("");
            await sut.AddNewProduct(new Product { Manufacturer = "Foo", ModelName = "Bar" });
        }

        [TestMethod]
        [ExpectedException(typeof(ParameterRequiredException))]
        public async Task UpdateProduct_WhenMissingManufacturer_ShouldThrowException()
        {
            ProductManagerMock sut = new ProductManagerMock("");
            await sut.UpdateProduct(new Product { ModelName = "Foo", Description = "Bar" });
        }

        [TestMethod]
        [ExpectedException(typeof(ParameterRequiredException))]
        public async Task UpdateProduct_WhenMissingModel_ShouldThrowException()
        {
            ProductManagerMock sut = new ProductManagerMock("");
            await sut.UpdateProduct(new Product { Manufacturer = "Foo", Description = "Bar" });
        }

        [TestMethod]
        [ExpectedException(typeof(ParameterRequiredException))]
        public async Task UpdateProduct_WhenMissingDescription_ShouldThrowException()
        {
            ProductManagerMock sut = new ProductManagerMock("");
            await sut.UpdateProduct(new Product { Manufacturer = "Foo", ModelName = "Bar" });
        }
    }
}
