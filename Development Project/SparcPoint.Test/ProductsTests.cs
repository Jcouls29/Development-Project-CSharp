using Interview.Web.Controllers;
using System;
using Sparcpoint.Repositories;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Assert = Xunit.Assert;
using System.Threading.Tasks;
using Sparcpoint.Entities;
using Moq;
using System.Collections.Generic;
using System.Net;
using System.Linq;

namespace SparcPoint.Test
{
    public class ProductsTests
    {
       
        [Fact]
        public async Task ProductRepository_GetProducts_Test()
        {

            //Given
            List<Product> products = new List<Product>()
                {
                new Product()
            {
                InstanceId = 1,
                Name = "Test Vanaja 1",
                Description = "Test Description",
                ProductImageUris ="Test ProductImageUris",
                ValidSkus = "Test Valid Skus",
                CreatedTimeStamp=DateTime.Now
            },
                 new Product()
            {
                InstanceId = 2,
                Name = "Test Vanaja 2",
                Description = "Test Description",
                ProductImageUris ="Test ProductImageUris",
                ValidSkus = "Test Valid Skus",
                CreatedTimeStamp=DateTime.Now
            },
                  new Product()
            {
                InstanceId = 3,
                Name = "Test Vanaja 3",
                Description = "Test Description",
                ProductImageUris ="Test ProductImageUris",
                ValidSkus = "Test Valid Skus",
                CreatedTimeStamp=DateTime.Now
            }
            };
            var mockProdctClient = new Mock<IProdcutRepository>();

            mockProdctClient.Setup(c => c.GetProducts()).ReturnsAsync((IEnumerable<Product>)products);


            //Arrange
            ProductController controller = new ProductController(mockProdctClient.Object);

            var result = await controller.GetProducts() as ObjectResult;
            var actualResult = result.Value;

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.Equal(HttpStatusCode.OK, (HttpStatusCode)result.StatusCode);
            mockProdctClient.Verify(c => c.GetProducts(), Times.Once);
            Assert.Equal(products[0].Name, ((IEnumerable<Product>)actualResult).FirstOrDefault(x=>x.Name==products[0].Name).Name);

        }

        [Fact]
        public async Task ProductRepository_CreateProduct_Test()
        {

            //Given
            var product = new Product()
            {
                InstanceId = 1,
                Name = "Test Vanaja 1",
                Description = "Test Description",
                ProductImageUris = "Test ProductImageUris",
                ValidSkus = "Test Valid Skus",
                CreatedTimeStamp = DateTime.Now
            };

            var productForCreationDto = new ProductForCreationDto()
            {
                Name = "Test Vanaja 1",
                Description = "Test Description",
                ProductImageUris = "Test ProductImageUris",
                ValidSkus = "Test Valid Skus"
            };
            
            var mockProdctClient = new Mock<IProdcutRepository>();

            mockProdctClient.Setup(c => c.CreateProduct(productForCreationDto)).ReturnsAsync(product);

            //Arrange
            ProductController controller = new ProductController(mockProdctClient.Object);

            var result = await controller.CreateProduct(productForCreationDto) as ObjectResult;
            var actualResult = result.Value;


            //Assert
            Assert.Equal(HttpStatusCode.Created, (HttpStatusCode)result.StatusCode);
            mockProdctClient.Verify(c => c.CreateProduct(productForCreationDto), Times.Once);
            Assert.Equal(product.InstanceId, ((Product)actualResult).InstanceId);


        }
    }
}
