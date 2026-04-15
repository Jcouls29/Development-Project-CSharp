using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Interview.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Core.DTOs;
using Sparcpoint.Core.Models;
using Sparcpoint.Core.Repositories;
using Xunit;

namespace Interview.Web.Tests
{
    public class CategoryControllerTests
    {
        private class FakeCategoryRepository : ICategoryRepository
        {
            private readonly Dictionary<int, Category> _categories = new Dictionary<int, Category>();
            private readonly Dictionary<int, List<int>> _parentRelations = new Dictionary<int, List<int>>();
            private int _nextId = 1;

            public Task<int> AddAsync(Category category)
            {
                var id = _nextId++;
                category.SetInstanceId(id);
                _categories[id] = category;
                _parentRelations[id] = new List<int>();
                return Task.FromResult(id);
            }

            public Task DeleteAsync(int instanceId)
            {
                _categories.Remove(instanceId);
                _parentRelations.Remove(instanceId);
                return Task.CompletedTask;
            }

            public Task<IEnumerable<Category>> GetAllAsync()
            {
                return Task.FromResult<IEnumerable<Category>>(_categories.Values.ToList());
            }

            public Task<Category> GetByIdAsync(int instanceId)
            {
                _categories.TryGetValue(instanceId, out var category);
                return Task.FromResult(category);
            }

            public Task<IEnumerable<int>> GetChildCategoryIdsAsync(int categoryInstanceId)
            {
                var children = _parentRelations.Where(kvp => kvp.Value.Contains(categoryInstanceId)).Select(kvp => kvp.Key).ToList();
                return Task.FromResult<IEnumerable<int>>(children);
            }

            public Task<IEnumerable<int>> GetParentCategoryIdsAsync(int categoryInstanceId)
            {
                _parentRelations.TryGetValue(categoryInstanceId, out var parents);
                return Task.FromResult<IEnumerable<int>>(parents ?? new List<int>());
            }

            public Task UpdateAsync(Category category)
            {
                _categories[category.InstanceId] = category;
                return Task.CompletedTask;
            }

            public Task UpdateParentCategoryRelationshipsAsync(int categoryInstanceId, IEnumerable<int> parentCategoryIds)
            {
                _parentRelations[categoryInstanceId] = parentCategoryIds?.ToList() ?? new List<int>();
                return Task.CompletedTask;
            }
        }

        [Fact]
        public async Task CreateCategory_ReturnsCreatedAtAction_WithInstanceId()
        {
            var repository = new FakeCategoryRepository();
            var controller = new CategoryController(repository);
            var createDto = new CreateCategoryDto
            {
                Name = "Electronics",
                Description = "Electronic devices",
                ParentCategoryIds = new List<int>()
            };

            var result = await controller.CreateCategory(createDto);
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(CategoryController.GetCategory), createdResult.ActionName);
            Assert.NotNull(createdResult.Value);
        }

        [Fact]
        public async Task GetCategory_ReturnsCategoryWithParentsAndChildren()
        {
            var repository = new FakeCategoryRepository();
            var parentCategory = Category.Create("Parent", "Parent category");
            var childCategory = Category.Create("Child", "Child category");
            var parentId = await repository.AddAsync(parentCategory);
            var childId = await repository.AddAsync(childCategory);
            await repository.UpdateParentCategoryRelationshipsAsync(childId, new[] { parentId });

            var controller = new CategoryController(repository);
            var result = await controller.GetCategory(childId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var categoryDto = Assert.IsType<CategoryDto>(okResult.Value);

            Assert.Contains(parentId, categoryDto.ParentCategoryIds);
            Assert.Empty(categoryDto.ChildCategoryIds);
        }
    }
}
