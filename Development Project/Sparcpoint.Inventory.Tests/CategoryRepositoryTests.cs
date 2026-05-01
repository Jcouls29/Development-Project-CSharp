using Moq;
using Sparcpoint.Inventory.Implementations;
using Sparcpoint.Inventory.Models;
using Sparcpoint.Inventory.Models.Requests;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Xunit;

namespace Sparcpoint.Inventory.Tests
{
    public class CategoryRepositoryTests
    {
        private readonly Mock<ISqlExecutor> _MockExecutor;
        private readonly SqlCategoryRepository _Repository;

        public CategoryRepositoryTests()
        {
            _MockExecutor = new Mock<ISqlExecutor>();
            _Repository = new SqlCategoryRepository(_MockExecutor.Object);
        }

        [Fact]
        public void Constructor_NullExecutor_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new SqlCategoryRepository(null));
        }

        [Fact]
        public async Task AddAsync_NullRequest_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _Repository.AddAsync(null));
        }

        [Fact]
        public async Task AddAsync_EmptyName_ThrowsArgumentException()
        {
            var request = new AddCategoryRequest { Name = "", Description = "A description" };
            await Assert.ThrowsAsync<ArgumentException>(() => _Repository.AddAsync(request));
        }

        [Fact]
        public async Task AddAsync_EmptyDescription_ThrowsArgumentException()
        {
            var request = new AddCategoryRequest { Name = "Electronics", Description = "" };
            await Assert.ThrowsAsync<ArgumentException>(() => _Repository.AddAsync(request));
        }

        [Fact]
        public async Task AddAsync_ValidRequest_DelegatesToExecutor()
        {
            _MockExecutor
                .Setup(e => e.ExecuteAsync<int>(It.IsAny<Func<IDbConnection, IDbTransaction, Task<int>>>()))
                .ReturnsAsync(3);

            var request = new AddCategoryRequest { Name = "Electronics", Description = "Electronic goods" };
            var result = await _Repository.AddAsync(request);

            Assert.Equal(3, result);
        }

        [Fact]
        public async Task GetAllAsync_DelegatesToExecutor()
        {
            _MockExecutor
                .Setup(e => e.ExecuteAsync<IEnumerable<Category>>(
                    It.IsAny<Func<IDbConnection, IDbTransaction, Task<IEnumerable<Category>>>>()))
                .ReturnsAsync(Array.Empty<Category>());

            var result = await _Repository.GetAllAsync();

            Assert.NotNull(result);
        }

        [Fact]
        public void Category_DefaultParentCategoryIds_IsEmpty()
        {
            // ParentCategoryIds must default to an empty collection so callers
            // can safely iterate without null checks
            var category = new Category { InstanceId = 1, Name = "Electronics", Description = "Desc" };
            Assert.NotNull(category.ParentCategoryIds);
            Assert.Empty(category.ParentCategoryIds);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsCategories_WithParentCategoryIdsNotNull()
        {
            var categories = new List<Category>
            {
                new Category { InstanceId = 1, Name = "Electronics", Description = "Desc" },
                new Category { InstanceId = 2, Name = "Phones", Description = "Desc" }
            };

            _MockExecutor
                .Setup(e => e.ExecuteAsync<IEnumerable<Category>>(
                    It.IsAny<Func<IDbConnection, IDbTransaction, Task<IEnumerable<Category>>>>()))
                .ReturnsAsync(categories);

            var result = await _Repository.GetAllAsync();

            // Each category returned must have a non-null ParentCategoryIds
            // (real population of parent IDs is covered by Bruno integration tests)
            foreach (var category in result)
                Assert.NotNull(category.ParentCategoryIds);
        }
    }
}
