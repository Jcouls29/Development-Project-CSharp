using Moq;
using Sparcpoint.Inventory.Interfaces;
using Sparcpoint.Inventory.Models;
using Sparcpoint.Inventory.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Interview.Tests
{
    public class CategoryServiceTests
    {
        #region CreateCategoryAsync

        [Fact]
        public async Task CreateCategoryAsync_ValidRequest_ReturnsCategory()
        {
            var request = new CreateCategoryRequest
            {
                Name = "Electronics",
                Description = "Electronic items",
            };

            var mockRepo = new Mock<ICategoryRepository>();
            mockRepo.Setup(r => r.CreateAsync(request)).ReturnsAsync(1);
            mockRepo.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new CategoryDetailModel
                {
                    InstanceId = 1,
                    Name = request.Name,
                    Description = request.Description,
                });

            var service = new CategoryService(mockRepo.Object);

            var result = await service.CreateCategoryAsync(request);

            Assert.NotNull(result);
            Assert.Equal("Electronics", result.Name);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task CreateCategoryAsync_NullOrWhitespaceName_ThrowsArgumentException(string? name)
        {
            var request = new CreateCategoryRequest
            {
                Name = name,
                Description = "Description",
            };

            var mockRepo = new Mock<ICategoryRepository>();
            var service = new CategoryService(mockRepo.Object);

            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateCategoryAsync(request));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task CreateCategoryAsync_NullOrWhitespaceDescription_ThrowsArgumentException(string? description)
        {
            var request = new CreateCategoryRequest
            {
                Name = "Name",
                Description = description,
            };

            var mockRepo = new Mock<ICategoryRepository>();
            var service = new CategoryService(mockRepo.Object);

            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateCategoryAsync(request));
        }

        [Fact]
        public async Task CreateCategoryAsync_NameTooLong_ThrowsArgumentException()
        {
            var request = new CreateCategoryRequest
            {
                Name = new string('a', 65),
                Description = "Valid description",
            };

            var mockRepo = new Mock<ICategoryRepository>();
            var service = new CategoryService(mockRepo.Object);

            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateCategoryAsync(request));
        }

        [Fact]
        public async Task CreateCategoryAsync_DescriptionTooLong_ThrowsArgumentException()
        {
            var request = new CreateCategoryRequest
            {
                Name = "Name",
                Description = new string('a', 257),
            };

            var mockRepo = new Mock<ICategoryRepository>();
            var service = new CategoryService(mockRepo.Object);

            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateCategoryAsync(request));
        }

        [Fact]
        public async Task CreateCategoryAsync_NullRequest_ThrowsArgumentNullException()
        {
            var mockRepo = new Mock<ICategoryRepository>();
            var service = new CategoryService(mockRepo.Object);

            await Assert.ThrowsAsync<ArgumentNullException>(() => service.CreateCategoryAsync(null!));
        }

        #endregion

        #region GetCategoryAsync

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetCategoryAsync_InvalidId_ThrowsArgumentOutOfRange(int invalidId)
        {
            var mockRepo = new Mock<ICategoryRepository>();
            var service = new CategoryService(mockRepo.Object);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.GetCategoryAsync(invalidId));
        }

        [Fact]
        public async Task GetCategoryAsync_ValidId_ReturnsCategory()
        {
            var mockRepo = new Mock<ICategoryRepository>();
            mockRepo.Setup(r => r.GetByIdAsync(42))
                .ReturnsAsync(new CategoryDetailModel { InstanceId = 42, Name = "Test" });

            var service = new CategoryService(mockRepo.Object);

            var result = await service.GetCategoryAsync(42);

            Assert.Equal(42, result.InstanceId);
        }

        #endregion

        #region GetCategoriesAsync

        [Theory]
        [InlineData(0, 10)]
        [InlineData(-1, 10)]
        public async Task GetCategoriesAsync_InvalidPage_ThrowsArgumentOutOfRange(int page, int pageSize)
        {
            var mockRepo = new Mock<ICategoryRepository>();
            var service = new CategoryService(mockRepo.Object);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.GetCategoriesAsync(page, pageSize));
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(1, -1)]
        public async Task GetCategoriesAsync_InvalidPageSize_ThrowsArgumentOutOfRange(int page, int pageSize)
        {
            var mockRepo = new Mock<ICategoryRepository>();
            var service = new CategoryService(mockRepo.Object);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.GetCategoriesAsync(page, pageSize));
        }

        #endregion

        #region AddAttributeAsync

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task AddAttributeAsync_InvalidId_ThrowsArgumentOutOfRange(int invalidId)
        {
            var mockRepo = new Mock<ICategoryRepository>();
            var service = new CategoryService(mockRepo.Object);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.AddAttributeAsync(invalidId, "key", "value"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task AddAttributeAsync_NullOrWhitespaceKey_ThrowsArgumentException(string? key)
        {
            var mockRepo = new Mock<ICategoryRepository>();
            var service = new CategoryService(mockRepo.Object);

            await Assert.ThrowsAsync<ArgumentException>(() => service.AddAttributeAsync(1, key, "value"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task AddAttributeAsync_NullOrWhitespaceValue_ThrowsArgumentException(string? value)
        {
            var mockRepo = new Mock<ICategoryRepository>();
            var service = new CategoryService(mockRepo.Object);

            await Assert.ThrowsAsync<ArgumentException>(() => service.AddAttributeAsync(1, "key", value));
        }

        #endregion

        #region RemoveAttributeAsync

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task RemoveAttributeAsync_InvalidId_ThrowsArgumentOutOfRange(int invalidId)
        {
            var mockRepo = new Mock<ICategoryRepository>();
            var service = new CategoryService(mockRepo.Object);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.RemoveAttributeAsync(invalidId, "key"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task RemoveAttributeAsync_NullOrWhitespaceKey_ThrowsArgumentException(string? key)
        {
            var mockRepo = new Mock<ICategoryRepository>();
            var service = new CategoryService(mockRepo.Object);

            await Assert.ThrowsAsync<ArgumentException>(() => service.RemoveAttributeAsync(1, key));
        }

        #endregion

        #region AddParentCategoryAsync

        [Theory]
        [InlineData(0, 1)]
        [InlineData(-1, 1)]
        public async Task AddParentCategoryAsync_InvalidInstanceId_ThrowsArgumentOutOfRange(int instanceId, int parentId)
        {
            var mockRepo = new Mock<ICategoryRepository>();
            var service = new CategoryService(mockRepo.Object);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.AddParentCategoryAsync(instanceId, parentId));
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(1, -1)]
        public async Task AddParentCategoryAsync_InvalidParentId_ThrowsArgumentOutOfRange(int instanceId, int parentId)
        {
            var mockRepo = new Mock<ICategoryRepository>();
            var service = new CategoryService(mockRepo.Object);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.AddParentCategoryAsync(instanceId, parentId));
        }

        #endregion

        #region RemoveParentCategoryAsync

        [Theory]
        [InlineData(0, 1)]
        [InlineData(-1, 1)]
        public async Task RemoveParentCategoryAsync_InvalidInstanceId_ThrowsArgumentOutOfRange(int instanceId, int parentId)
        {
            var mockRepo = new Mock<ICategoryRepository>();
            var varvice = new CategoryService(mockRepo.Object);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => varvice.RemoveParentCategoryAsync(instanceId, parentId));
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(1, -1)]
        public async Task RemoveParentCategoryAsync_InvalidParentId_ThrowsArgumentOutOfRange(int instanceId, int parentId)
        {
            var mockRepo = new Mock<ICategoryRepository>();
            var service = new CategoryService(mockRepo.Object);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.RemoveParentCategoryAsync(instanceId, parentId));
        }

        #endregion

        #region Constructor

        [Fact]
        public void Constructor_NullRepository_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new CategoryService(null!));
        }

        #endregion
    }
}