using System;
using System.Threading.Tasks;
using Interview.Web.Models;
using Interview.Web.Repositories;
using Interview.Web.Services;
using Moq;
using Xunit;

namespace Interview.Web.Tests
{
    public class CategoryServiceTests
    {
        [Fact]
        public async Task AddAsync_CallsRepositoryAndReturnsCategory()
        {
            var mockRepo = new Mock<ICategoryRepository>();
            Category saved = null;
            mockRepo.Setup(r => r.AddAsync(It.IsAny<Category>())).ReturnsAsync((Category c) => { saved = c; return c; });

            var svc = new CategoryService(mockRepo.Object);

            var category = new Category { Name = "Test", DisplayName = "Test Cat" };

            var result = await svc.AddAsync(category);

            Assert.NotNull(result);
            Assert.Equal("Test", result.Name);
            mockRepo.Verify(r => r.AddAsync(It.IsAny<Category>()), Times.Once);
        }
    }
}
