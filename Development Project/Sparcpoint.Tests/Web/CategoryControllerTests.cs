using System.Collections.Generic;
using System.Threading.Tasks;
using Interview.Web.Controllers;
using Interview.Web.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Sparcpoint.Inventory;
using Sparcpoint.Inventory.Models;
using Xunit;

namespace Sparcpoint.Tests.Web
{
    public class CategoryControllerTests
    {
        private static CategoryController Build(ICategoryRepository repo)
            => new CategoryController(repo);

        private static Mock<ICategoryRepository> MockRepo() => new Mock<ICategoryRepository>();

        // ── GetAll ────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetAll_ReturnsOkWithAllCategories()
        {
            var categories = new List<CategoryEntry>
            {
                new CategoryEntry { InstanceId = 1, Name = "Electronics" },
                new CategoryEntry { InstanceId = 2, Name = "Office"      },
            };
            var repo = MockRepo();
            repo.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);

            var result = await Build(repo.Object).GetAll();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Same(categories, ok.Value);
        }

        // ── GetById ───────────────────────────────────────────────────────────

        [Fact]
        public async Task GetById_CategoryExists_ReturnsOk()
        {
            var category = new CategoryEntry { InstanceId = 1, Name = "Electronics" };
            var repo = MockRepo();
            repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(category);

            var result = await Build(repo.Object).GetById(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Same(category, ok.Value);
        }

        [Fact]
        public async Task GetById_CategoryNotFound_ReturnsNotFound()
        {
            var repo = MockRepo();
            repo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((CategoryEntry?)null);

            var result = await Build(repo.Object).GetById(99);

            Assert.IsType<NotFoundResult>(result);
        }

        // ── Create ────────────────────────────────────────────────────────────

        [Fact]
        public async Task Create_ValidRequest_ReturnsCreatedAtAction()
        {
            var repo = MockRepo();
            repo.Setup(r => r.AddAsync(It.IsAny<CategoryEntry>())).ReturnsAsync(3);

            var result = await Build(repo.Object).Create(new CreateCategoryRequest { Name = "Electronics" });

            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(CategoryController.GetById), created.ActionName);
        }

        [Fact]
        public async Task Create_SetsNameAndDescriptionOnEntry()
        {
            var repo = MockRepo();
            repo.Setup(r => r.AddAsync(It.IsAny<CategoryEntry>())).ReturnsAsync(1);

            await Build(repo.Object).Create(new CreateCategoryRequest
            {
                Name        = "Electronics",
                Description = "Electronic goods",
            });

            repo.Verify(r => r.AddAsync(It.Is<CategoryEntry>(c =>
                c.Name == "Electronics" && c.Description == "Electronic goods")), Times.Once);
        }

        [Fact]
        public async Task Create_NullRequest_ReturnsBadRequest()
        {
            var result = await Build(MockRepo().Object).Create(null!);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Create_MissingName_ReturnsBadRequest()
        {
            var result = await Build(MockRepo().Object).Create(new CreateCategoryRequest { Name = "" });

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Create_SetsParentCategoryIds()
        {
            var repo = MockRepo();
            repo.Setup(r => r.AddAsync(It.IsAny<CategoryEntry>())).ReturnsAsync(1);

            await Build(repo.Object).Create(new CreateCategoryRequest
            {
                Name              = "Sub",
                ParentCategoryIds = new[] { 1, 2 },
            });

            repo.Verify(r => r.AddAsync(It.Is<CategoryEntry>(c =>
                c.ParentCategoryIds.Length == 2)), Times.Once);
        }
    }
}
