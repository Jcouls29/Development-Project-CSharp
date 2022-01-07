using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sparcpoint.DataServices;
using Sparcpoint.Models;
using Sparcpoint.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sparcpoint.Test
{
    [TestClass]
    public class TheCategoryService
    {
        [TestMethod]
        public async Task GetsAllCurrentCategories()
        {
            var expectedReturnList = new List<Category>();

            Mock<ICategoryDataService> mockDataService = new Mock<ICategoryDataService>();
            mockDataService.Setup(p => p.GetCategories()).Returns(Task.FromResult(expectedReturnList));
            var categoryService = new CategoryService(mockDataService.Object);
            var result = await categoryService.GetCategories();
            Assert.AreEqual(expectedReturnList, result);
        }

        [TestClass]
        public class WhenCreatingACategory
        {

            [TestMethod]
            public async Task SavesACategoryToTheDataService()
            {
                Category calledWith = new Category();
                Mock<ICategoryDataService> mockDataService = new Mock<ICategoryDataService>();
                mockDataService.Setup(p => p.CreateCategoryAsync(It.IsAny<Category>())).Callback<Category>(c => calledWith = c).Returns(Task.FromResult(5));
                var categoryService = new CategoryService(mockDataService.Object);

                var categoryRequest = new CreateCategoryRequest()
                {
                    Name = "First Category",
                    Description = "A test description"
                };

                await categoryService.CreateCategoryAsync(categoryRequest);

                Assert.AreEqual(categoryRequest.Name, calledWith.Name);
                Assert.AreEqual(categoryRequest.Description, calledWith.Description);

            }

            [TestClass]
            public class GivenTheModelContainsAttributes
            {
                [TestMethod]
                public async Task CallsToAddTheAttributesToTheCategory()
                {
                    List<KeyValuePair<string, string>> calledWith = new List<KeyValuePair<string, string>>();
                    Mock<ICategoryDataService> mockDataService = new Mock<ICategoryDataService>();
                    mockDataService.Setup(p => p.CreateCategoryAsync(It.IsAny<Category>())).Returns(Task.FromResult(5));
                    mockDataService.Setup(p => p.AddAttributeToCategory(It.IsAny<int>(), It.IsAny<KeyValuePair<string,string>>())).Callback<int, KeyValuePair<string, string>>((id, attribute)=> calledWith.Add(attribute));
                    var categoryService = new CategoryService(mockDataService.Object);

                    var categoryRequest = new CreateCategoryRequest()
                    {
                        Name = "First Category",
                        Description = "A test description",
                        CategoryAttributes = new List<KeyValuePair<string, string>>() { 
                            new KeyValuePair<string, string>("Brand", "Samsung"),
                            new KeyValuePair<string, string>("Color", "Red")
                        }
                    };

                    await categoryService.CreateCategoryAsync(categoryRequest);

                    mockDataService.Verify(d => d.AddAttributeToCategory(5, It.IsAny<KeyValuePair<string, string>>()), Times.Exactly(2));
                    Assert.AreEqual(categoryRequest.CategoryAttributes.ElementAt(0).Key, calledWith.ElementAt(0).Key);
                    Assert.AreEqual(categoryRequest.CategoryAttributes.ElementAt(0).Value, calledWith.ElementAt(0).Value);
                    Assert.AreEqual(categoryRequest.CategoryAttributes.ElementAt(1).Key, calledWith.ElementAt(1).Key);
                    Assert.AreEqual(categoryRequest.CategoryAttributes.ElementAt(1).Value, calledWith.ElementAt(1).Value);
                }
            }

            [TestClass]
            public class GivenTheModelDoesNotContainAttributes
            {
                [TestMethod]
                public async Task DoesNotCallToAddTheAttributesToTheCategory()
                {
                    Mock<ICategoryDataService> mockDataService = new Mock<ICategoryDataService>();
                    mockDataService.Setup(p => p.CreateCategoryAsync(It.IsAny<Category>())).Returns(Task.FromResult(5));
                    mockDataService.Setup(p => p.AddAttributeToCategory(It.IsAny<int>(), It.IsAny<KeyValuePair<string, string>>())).Verifiable();
                    var categoryService = new CategoryService(mockDataService.Object);

                    var categoryRequest = new CreateCategoryRequest()
                    {
                        Name = "First Category",
                        Description = "A test description"
                    };

                    await categoryService.CreateCategoryAsync(categoryRequest);

                    mockDataService.Verify(d => d.AddAttributeToCategory(It.IsAny<int>(), It.IsAny<KeyValuePair<string, string>>()), Times.Never());
                }
            }

            [TestClass]
            public class GivenTheModelContainsCategories
            {
                [TestMethod]
                public async Task CallsToAddCateogiresToCategory()
                {
                    Mock<ICategoryDataService> mockDataService = new Mock<ICategoryDataService>();
                    mockDataService.Setup(p => p.CreateCategoryAsync(It.IsAny<Category>())).Returns(Task.FromResult(5));
                    mockDataService.Setup(p => p.AddCategoryToCategory(It.IsAny<int>(), It.IsAny<int>())).Verifiable();
                    var categoryService = new CategoryService(mockDataService.Object);

                    var categoryRequest = new CreateCategoryRequest()
                    {
                        Name = "First Category",
                        Description = "A test description",
                        CategoryIds = new List<int>() { 3, 6, 7}
                    };

                    await categoryService.CreateCategoryAsync(categoryRequest);

                    mockDataService.Verify(d => d.AddCategoryToCategory(5, 3), Times.Once());
                    mockDataService.Verify(d => d.AddCategoryToCategory(5, 6), Times.Once());
                    mockDataService.Verify(d => d.AddCategoryToCategory(5, 7), Times.Once());

                }
            }

            [TestClass]
            public class GivenTheModelDoesNotContainCategories
            {
                [TestMethod]
                public async Task DoesNotCallToAddTheAttributesToTheCategory()
                {
                    Mock<ICategoryDataService> mockDataService = new Mock<ICategoryDataService>();
                    mockDataService.Setup(p => p.CreateCategoryAsync(It.IsAny<Category>())).Returns(Task.FromResult(5));
                    mockDataService.Setup(p => p.AddCategoryToCategory(It.IsAny<int>(), It.IsAny<int>())).Verifiable();
                    var categoryService = new CategoryService(mockDataService.Object);

                    var categoryRequest = new CreateCategoryRequest()
                    {
                        Name = "First Category",
                        Description = "A test description"
                    };

                    await categoryService.CreateCategoryAsync(categoryRequest);

                    mockDataService.Verify(d => d.AddCategoryToCategory(It.IsAny<int>(), It.IsAny<int>()), Times.Never());
                }
            }

        }
    }
}
