using AutoMapper;
using Interview.Web.Controllers;
using Interview.Web.Dtos;
using Interview.Web.Mapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Sparcpoint.Interfaces;
using Sparcpoint.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sparcpoint.Tests
{
    [TestFixture]
    public class ProductControllerTests
    {
        [Test]
        public void GetProducts_Should_Return_List_of_Product_Dtos()
        {
            // Arrange
            IEnumerable<Product> data = new List<Product> 
            { 
                new Product { InstanceId = 1, Name = "Printer", Description = "Laser Printer" } 
            };
            var mockUow = new Mock<IUnitOfWork>();
            mockUow
                .Setup(x => x.ProductsRepository.GetProductsAsync())
                .Returns(Task.FromResult(data));

            var profile = new AutoMapperProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(profile));
            IMapper mapper = new Mapper(configuration);

            var controller = new ProductController(mockUow.Object, mapper);

            // Act
            var actionResult = controller.GetAllProducts().Result;
            var result = actionResult as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(1, ((IEnumerable<ProductDto>)result.Value).ToList().Count);
        }
    }
    
}
