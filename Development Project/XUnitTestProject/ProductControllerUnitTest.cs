using Interview.Data.ViewModels;
using Interview.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace XUnitTestProject
{
    public class ProductControllerUnitTest
    {
        [Fact]
        public async System.Threading.Tasks.Task GetAllProducts()
        {
            // Arrange
            var mockRepo = new Mock<Interview.Web.Repository.IGenericRepository<ProductViewModel>>();
            mockRepo.Setup(repo => repo.GetAll()).ReturnsAsync(GetTestData());
            var controller = new ProductController(mockRepo.Object);
            
            // Act
            var result = await controller.GetAllProducts();
            var okResult = result as OkObjectResult;

            // assert
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
        }

        private List<ProductViewModel> GetTestData()
        {
            return new List<ProductViewModel>() {
                new ProductViewModel() 
                {
                    Name = "Test1",
                    Description ="TestDesc",
                    ProductAttributes = new List<GenericAttribute>() 
                    { 
                        new GenericAttribute() { Key ="key1", Value="Va1" },
                        new GenericAttribute() { Key ="key1", Value="Va1" }
                    },
                    ProductImageUris = "www.google.com",
                    ValidSkus = "valid",
                    Categories = new List<CategoryViewModel>()
                    { 
                        new CategoryViewModel() { 
                            Name = "name", Description ="desc", 
                            CategoryAttributes =  new List<GenericAttribute>() 
                            {
                                new GenericAttribute() { Key ="key1", Value="Va1" },
                                new GenericAttribute() { Key ="key1", Value="Va1" }
                            }
                        }
                    }
                },
                new ProductViewModel()
                {
                    Name = "Test2",
                    Description ="TestDesc",
                    ProductAttributes = new List<GenericAttribute>()
                    {
                        new GenericAttribute() { Key ="key1", Value="Va1" },
                        new GenericAttribute() { Key ="key1", Value="Va1" }
                    },
                    ProductImageUris = "www.google.com",
                    ValidSkus = "valid",
                    Categories = new List<CategoryViewModel>()
                    {
                        new CategoryViewModel() {
                            Name = "name", Description ="desc",
                            CategoryAttributes =  new List<GenericAttribute>()
                            {
                                new GenericAttribute() { Key ="key1", Value="Va1" },
                                new GenericAttribute() { Key ="key1", Value="Va1" }
                            }
                        }
                    }
                }
            };
        }
    }
}
