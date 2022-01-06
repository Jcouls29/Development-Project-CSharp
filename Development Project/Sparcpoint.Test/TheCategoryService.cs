using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sparcpoint.DataServices;
using Sparcpoint.Models;
using Sparcpoint.Services;
using System;
using System.Collections.Generic;
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

        [TestMethod]
        public async Task SavesACategoryToTheDataService()
        {
            Category calledWith = new Category();
            Mock<ICategoryDataService> mockDataService = new Mock<ICategoryDataService>();
            mockDataService.Setup(p => p.CreateCategoryAsync(It.IsAny<Category>())).Callback<Category>(c => calledWith = c);
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
    }
}
