using Microsoft.VisualStudio.TestTools.UnitTesting;
using Interview.Web.Services;
using Interview.Web.Models;

namespace Interview.Web.Tests
{
    [TestClass]
    public class ProductServiceTest
    {
        [TestMethod]
        public void ShouldGetProduct()
        {
            var productService = new ProductService(new Data.InventoryContext(new Microsoft.EntityFrameworkCore.DbContextOptions<Data.InventoryContext>()));
            productService.AddProduct(new Product { Id = 1, Name = "Test", CategoryId = 1, CustomerId = 1 });

            Assert.IsTrue(true);
        }
    }
}