using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sparcpoint.Product.Domain;
using Sparcpoint.Tests.UnitTests.Mocks;
using System;

namespace Sparcpoint.Tests.UnitTests
{
    [TestClass]
    public class ProductManagerTests
    {
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void GetProductById_WhenProductNotExists_ShouldThrowException()
        {
            // Eval: The advantage here is that we can now use straightforward inheritance to create our tests.
            // Tests become more readable and easier to understand.
            InventoryManagerMock sut = new InventoryManagerMock("");
            sut.GetProductById(0);
        }
    }
}
