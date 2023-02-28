using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using MockQueryable.Moq;
using Moq;
using Moq.EntityFrameworkCore;
using Shouldly;
using Sparcpoint.Inventory;
using Sparcpoint.Inventory.Models;
using Sparcpoint.Inventory.Repostiories;

namespace SparcPoint.Inventory.Tests
{
    public class ProductRepositoryTests
    {
        [Fact]
        public void ShouldGetAll()
        {
            var mock = TestDataHelper.GetFakeProductList().BuildMock().BuildMockDbSet();
            var mockContext = new Mock<SparcpointInventoryDatabaseContext>();
            
            mockContext.Setup(x => x.Products).Returns(mock.Object);
            mockContext.Setup(x => x.Set<Product>()).Returns(mock.Object);

            var sut = new ProductRepository(mockContext.Object);
            
            var products = sut.GetAll();

            products.ShouldNotBeNull();
        }

        [Fact]
        public async void ShouldGetAllAsync()
        {
            var mock = TestDataHelper.GetFakeProductList().BuildMock().BuildMockDbSet();
            var mockContext = new Mock<SparcpointInventoryDatabaseContext>();

            mockContext.Setup(x => x.Products).Returns(mock.Object);
            mockContext.Setup(x => x.Set<Product>()).Returns(mock.Object);

            var sut = new ProductRepository(mockContext.Object);

            var products = await sut.GetAllAsync();

            products.Count().ShouldBe(3);
        }

        [Fact]
        public void ShouldGetById()
        {
            var mock = TestDataHelper.GetFakeProductList().BuildMock().BuildMockDbSet();
            mock.Setup(x => x.Find(0)).Returns(TestDataHelper.GetFakeProductList().Find(x => x.InstanceId == 0));
            var mockContext = new Mock<SparcpointInventoryDatabaseContext>();
            
            mockContext.Setup(x => x.Set<Product>()).Returns(mock.Object);

            var sut = new ProductRepository(mockContext.Object);

            var products = sut.Get(0);

            products.ShouldNotBeNull();
        }

        [Fact]
        public async void ShouldGetAsync()
        {
            var mock = TestDataHelper.GetFakeProductList().BuildMock().BuildMockDbSet();
            mock.Setup(x => x.FindAsync(0)).ReturnsAsync(TestDataHelper.GetFakeProductList().Find(x => x.InstanceId == 0));
            var mockContext = new Mock<SparcpointInventoryDatabaseContext>();

            mockContext.Setup(x => x.Set<Product>()).Returns(mock.Object);

            var sut = new ProductRepository(mockContext.Object);

            var products = await sut.GetAsync(0);

            products.ShouldNotBeNull();
        }

        [Fact]
        public void ShouldAdd()
        {
            var fakeProduct = TestDataHelper.GetFakeProduct();

            var mock = TestDataHelper.GetFakeProductList().BuildMock().BuildMockDbSet();

            var mockContext = new Mock<SparcpointInventoryDatabaseContext>();

            mockContext.Setup(x => x.Set<Product>()).Returns(mock.Object);

            var sut = new ProductRepository(mockContext.Object);

            var result = sut.Add(fakeProduct);

            result.ShouldNotBeNull();
        }

        [Fact]
        public void ShouldFindAll()
        {
            var mock = TestDataHelper.GetFakeProductList().BuildMock().BuildMockDbSet();

            var mockContext = new Mock<SparcpointInventoryDatabaseContext>();

            mockContext.Setup(x => x.Set<Product>()).Returns(mock.Object);

            var sut = new ProductRepository(mockContext.Object);

            sut.ShouldNotBeNull();
        }
    }

    public static class TestDataHelper
    {
        public static Product GetFakeProduct()
        {
            return new Product
            {
                InstanceId = 3,
                Name = "Test003",
                Description = "Description003",
                ProductImageUris = "Uris003",
                ValidSkus = "Skues003",
                CreatedTimestamp = DateTime.Now,
                ProductAttributes = { },
                CategoryInstances = { },
                InventoryTransactions = { }
            };
        }

        public static List<Product> GetFakeProductList()
        {
            return new List<Product>
            {
                new Product
                {
                    InstanceId = 0,
                    Name = "Name001",
                    Description = "Description001",
                    ProductImageUris = "Uris001",
                    ValidSkus = "Skus001",
                    CreatedTimestamp = DateTime.Now,
                    ProductAttributes = { },
                    CategoryInstances = { },
                    InventoryTransactions = { }
                },
                new Product
                {
                    InstanceId = 1,
                    Name = "Name002",
                    Description = "Description002",
                    ProductImageUris = "Uris002",
                    ValidSkus = "Skus002",
                    CreatedTimestamp = DateTime.Now,
                    ProductAttributes = { },
                    CategoryInstances = { },
                    InventoryTransactions = { }
                },
                new Product
                {
                    InstanceId = 2,
                    Name = "Name003",
                    Description = "Description003",
                    ProductImageUris = "Uris003",
                    ValidSkus = "Skus003",
                    CreatedTimestamp = DateTime.Now,
                    ProductAttributes = { },
                    CategoryInstances = { },
                    InventoryTransactions = { }
                },
            };
        }
    }
}