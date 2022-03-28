using Interview.Web.Controllers;
using Interview.Web.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Interview.Web.CustomModels;

namespace Interview.Api.Tests
{
    [TestFixture]
    public class ProductControllerTests
    {
        private ProductController _prodController;

        [SetUp]
        public void SetUp()
        {
            TestManager tm = new();
            var logger = tm.LoggerFactory.CreateLogger<ProductController>();
            var _prodRepo = Mock.Of<IProductRepo>();
            _prodController = new(_prodRepo, logger);
        }
        [Test]
        public void Should_Search_For_Product()
        {
            //Arrange
            var inputMock = new Mock<SearchInput>().Object;
            //Act
            var result = _prodController.SearchProducts(inputMock);
            //Assert
            Assert.IsTrue(result.IsCompleted);
        }
    }

}
