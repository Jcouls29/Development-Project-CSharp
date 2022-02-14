using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sparcpoint.Application.Abstracts;
using Sparcpoint.Application.Implementations;
using Sparcpoint.Domain.Instance.Entities;
using Sparcpoint.Domain.Requestes;
using Sparcpoint.SqlServer.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sparcpoint.Test
{
    [TestClass]
    public class ProductServiceTests
    {

        private Mock<SqlServerQueryProvider> _mockSqlServerQueryProvider = new Mock<SqlServerQueryProvider>();
        private Mock<IQueryService> _mockQueryService = new Mock<IQueryService>();
        private Mock<ISqlExecutor> _mockSqlExecutor = new Mock<ISqlExecutor>();

        [TestMethod]
        public async Task GetsProducts()
        {
            var expectedReturnList = new List<Product>();
            Mock<IProductService> mockDataService = new Mock<IProductService>();
            mockDataService.Setup(p => p.GetProducts()).Returns(Task.FromResult(expectedReturnList));
             var productService = new ProductService(_mockSqlServerQueryProvider.Object, _mockQueryService.Object, _mockSqlExecutor.Object);
            var result = await productService.GetProducts();

            Assert.AreEqual(expectedReturnList, result);
        }
        [TestMethod]
        public async Task SavesAProductToTheDataService()
        {
            Product calledWith = new Product();
            Mock<IProductService> mockDataService = new Mock<IProductService>();
            mockDataService.Setup(p => p.CreateProductAsync(It.IsAny<Product>())).Callback<Product>(p => calledWith = p);
            var productService = new ProductService(_mockSqlServerQueryProvider.Object, _mockQueryService.Object, _mockSqlExecutor.Object);

            var product = new Product()
            {
                Name = "iPhone X",
                Description = "IPhone X description",
                ProductImageUris = "icon1.jpeg, icon2.jpg",
                ValidSkus = "1233, 5678", 
            };

            await productService.CreateProductAsync(product);

            Assert.AreEqual(product.Name, calledWith.Name);
            Assert.AreEqual(product.Description, calledWith.Description);
           
        }


    }


    }