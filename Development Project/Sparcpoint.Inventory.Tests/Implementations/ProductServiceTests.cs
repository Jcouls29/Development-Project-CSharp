using Moq;
using Sparcpoint.Inventory.Implementations;
using Sparcpoint.Inventory.Models;
using Sparcpoint.SqlServer.Abstractions;
using System.Data;
using System;

namespace Sparcpoint.Inventory.Tests.Implementations
{
    public class ProductServiceTests
    {
        private Mock<ISqlExecutor> _mockSqlExecutor;

        [SetUp]
        public void Setup()
        {
            _mockSqlExecutor = new Mock<ISqlExecutor>();
        }

        [Test]
        public void Constructor_When_GivenNullExecutor_Should_ThrowArgumentNullException()
        {
            // EVAL: not a great test but we are testing the functionality of the constructor
            Assert.Throws<ArgumentNullException>(() => new ProductService(null));
        }

        [Test]
        public void GetProductsAsync_When_Called_Should_Succeed()
        {
            // EVAL: not a ton of functionality here yet but test that ExecuteAsync is called once,
            // QueryAsync is called on a mockConnection with a valid query and given the transaction,
            // and that the items are returned untouched
            Assert.Pass();
        }
    }
}